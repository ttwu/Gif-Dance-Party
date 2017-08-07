using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the animated gif url options.  It both controls what will
/// show in the preview and will track which animated gifs get instantiated on
/// the board.
/// As the user scrolls, this manager will update the preview.  When the user
/// chooses to put a dancer on the board, it will instantiate another instance of it.
/// This class is smart enough to use only one AnimatedGifTexture component for instances
/// of the same gif url.  But it is not smart enough yet to track when you have removed
/// all instances of one from the board to disable that component.
/// </summary>
public class GifManager : MonoBehaviour
{
	private Dictionary<string, AnimatedGifTexture> animatedTextureCompCache;
	private string[] urls;
	private int currentGifIndex = 0;
	private AnimatedGifTexture currentAnimatedGifComponent;

	[SerializeField]
	private UnityEngine.UI.Image previewImage;
	[SerializeField]
	private Transform locatorGroup;
	private Transform[] locators;
	private int currentLocatorIndex = 0;
	[SerializeField]
	private GameObject animatedGifPrefabGameObject;
	[SerializeField]
	private Material animatedGifMtl;

	/// <summary>
	/// Initialize the dictionary and set a starting gif url for the preview.
	/// </summary>
	private void Start()
	{
		animatedTextureCompCache = new Dictionary<string, AnimatedGifTexture>();
		urls = GiphyQuery.GetGifUrls(0);
		currentGifIndex = 0;

		//get locators where gifs will get instantiated
		var numLocators = locatorGroup.GetChildCount();
		locators = new Transform[numLocators];
		for(var i=0; i<numLocators; i++)
		{
			locators[i] = locatorGroup.GetChild(i);
		}
	}

	/// <summary>
	/// The first time gif preview pops up, it needs to load up the first animated gif
	/// </summary>
	public void InitializePreview()
	{
		var currentUrl = urls[currentGifIndex];
		LoadUrlIntoPreview(currentUrl);
	}

	/// <summary>
	/// Given a url it will it will make or reuse a cached animated gif component for it in the preview.
	/// </summary>
	/// <param name="currentUrl"></param>
	void LoadUrlIntoPreview(string currentUrl)
	{
		//TODO - need smarts to know when to disable an animating component when there are no more instances of it on the board

		if (!animatedTextureCompCache.ContainsKey(currentUrl))
		{
			currentAnimatedGifComponent = gameObject.AddComponent<AnimatedGifTexture>();
			currentAnimatedGifComponent.LoadAndShowGif(currentUrl, previewImage);
			animatedTextureCompCache.Add(currentUrl, currentAnimatedGifComponent);
		}
		else
		{
			currentAnimatedGifComponent = animatedTextureCompCache[currentUrl];
		}
	}

	/// <summary>
	/// Update the preview gif when user scrolls the gif selection.
	/// </summary>
	/// <param name="indexStep"></param>
	public void ScrollGifSelection(int indexStep)
	{
		currentGifIndex = (currentGifIndex + indexStep) % urls.Length;
		if (currentGifIndex < 0) currentGifIndex += urls.Length;

		var currentGifUrl = urls[currentGifIndex];
		LoadUrlIntoPreview(currentGifUrl);
	}

	/// <summary>
	/// Instantiates the current animated gif in the preview at the next locator position.
	/// TODO - make it so the user can specify a spot on the grid to instantiate instead of predefined locations.
	/// </summary>
	public void InstantiateCurrentGif()
	{
		var currentLocator = locators[currentLocatorIndex];
		UnityEngine.UI.Image imageAtLocator = currentLocator.GetComponent<UnityEngine.UI.Image>();

		//TODO - support replacing existing gifs in the future
		//for now, if an animated gif is already there, just bail
		if (imageAtLocator != null) return;

		var newGifGO = Instantiate<GameObject>(animatedGifPrefabGameObject, currentLocator);
		imageAtLocator = newGifGO.GetComponent<UnityEngine.UI.Image>();
		var cachedAnimGifComponent = animatedTextureCompCache[urls[currentGifIndex]];
		imageAtLocator.material = new Material(animatedGifMtl);
		cachedAnimGifComponent.AddImageToUpdate(imageAtLocator);
		
		currentLocatorIndex = (currentLocatorIndex + 1)%locators.Length;
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the animated gif url options.  It both controls what will
/// show in the preview and will track which animated gifs get instantiated on
/// the board.
/// </summary>
public class GifManager : MonoBehaviour
{
	//track Textures that are currently being used 
	private Dictionary<string, AnimatedGifTexture> animatedTextureCompCache;
	public List<Texture2D> textureCache;
	//url results to page through
	private string[] urls;
	private int currentGifIndex = 0;

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
		textureCache = new List<Texture2D>();
		//texture2DCache = new Dictionary<string, Texture2D>();
		animatedTextureCompCache = new Dictionary<string, AnimatedGifTexture>();
		urls = GiphyQuery.GetGifUrls(0);
		//previewGif.SetGifUrl(urls[0]);
		var numLocators = locatorGroup.GetChildCount();
		locators = new Transform[numLocators];
		for(var i=0; i<numLocators; i++)
		{
			locators[i] = locatorGroup.GetChild(i);
		}
	}

	public void InitializePreview()
	{
		var currentUrl = urls[currentGifIndex];
		if (!animatedTextureCompCache.ContainsKey(currentUrl))
		{
			var newComponent = gameObject.AddComponent<AnimatedGifTexture>();
			newComponent.LoadAndShowGif(currentUrl, previewImage);
			/*
			animatedTextureCompCache.Add(currentUrl, newComponent);
			*/
		}
	}

	/// <summary>
	/// Update the preview gif when user scrolls the gif selection.
	/// </summary>
	/// <param name="indexStep"></param>
	public void ScrollGifSelection(int indexStep)
	{
		CacheCurrentTexture();

		currentGifIndex = (currentGifIndex + indexStep) % urls.Length;
		if (currentGifIndex < 0) currentGifIndex += urls.Length;

		var currentGifUrl = urls[currentGifIndex];
		//if (texture2DCache.ContainsKey(currentGifUrl))

		//previewGif.LoadAndShowGif(currentGifUrl);
	}

	void CacheCurrentTexture()
	{
		var currentUrl = urls[currentGifIndex];
		if (!animatedTextureCompCache.ContainsKey(currentUrl))
		{
			//texture2DCache.Add(currentUrl, previewGif.GetTexture2D());
			//animatedTextureCompCache.Add(currentUrl, previewGif);
			//textureCache.Add(previewGif.GetTexture2D());
		}
	}

	/// <summary>
	/// Instantiates the current animated gif in the preview at the next locator position.
	/// TODO - make it so the user can specify a spot on the grid to instantiate instead of predefined locations.
	/// </summary>
	public void InstantiateCurrentGif()
	{
		//if the target locator already has an animated gif, replace it
		var currentLocator = locators[currentLocatorIndex];
		AnimatedGifTexture gifTexture = currentLocator.GetComponentInChildren<AnimatedGifTexture>();
		if (gifTexture == null)
		{
			var newGifGO = Instantiate<GameObject>(animatedGifPrefabGameObject, currentLocator);
			gifTexture = newGifGO.GetComponent<AnimatedGifTexture>();
		}
		//var tex = previewGif.GetTexture2D();
		if (!animatedTextureCompCache.ContainsKey(urls[currentGifIndex]))
		{
			//animatedTextureCompCache.Add(urls[currentGifIndex], tex);
		}
		//gifTexture.UseThisAnimatedGif(previewGif, tex);  //RESUME HERE!!
		currentLocatorIndex = (currentLocatorIndex + 1)%locators.Length;
	}
}

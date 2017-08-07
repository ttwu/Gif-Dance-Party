using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System;

/// <summary>  
///  This component loads an animated gif from the web, and creates a Texture2D with tiled frames.
///  Upon Update it will update the UVs of the texture accordingly to animate the texture.
///  TODO 
///  - detect when animated gif is too big to store in just one texture and support multi-texture
///  - more optimization by having texture dimensions be powers of 2?
///  - pull animation timing information from gif (check here https://social.microsoft.com/Forums/en-US/fcb7d14d-d15b-4336-971c-94a80e34b85e/editing-animated-gifs-in-c?forum=netfxbcl)
/// </summary>  
public class AnimatedGifTexture : MonoBehaviour {
	private UnityWebRequest webRequest;
	private byte[] imageBuffer;
	private int numberOfFrames = 0;

	//TODO - use an array in case a large animated gif causes us to run out of space on one Texture2D
	public Texture2D texture;
	private HashSet<UnityEngine.UI.Image> uiImagesToUpdate;
	private RectTransform rectTransform;

	public int FramesPerSecond = 10;
	private float timeUpdateStep = 0f;
	private float currentXTileOffset = 0f;
	private float offsetStep = 0f;
	private float timeOfLastUpdate = 0;
	private string gifUrl;

	private void Awake()
	{
		uiImagesToUpdate = new HashSet<UnityEngine.UI.Image>();
	}

	/// <summary>  
	/// Load gif from new url, sets textures and materials.
	/// </summary>  
	public void LoadAndShowGif(string newUrl, UnityEngine.UI.Image targetImage)
	{
		gifUrl = newUrl;
		if (!uiImagesToUpdate.Contains(targetImage))
		{
			uiImagesToUpdate.Add(targetImage);
		}
		if (texture == null)
		{
			LoadAndShowGif();
		}
	}


	/// <summary>  
	/// Load gif from current url, sets textures and materials.
	/// TODO - test with invalid links or png/jpg
	/// </summary>  
	public void LoadAndShowGif()
	{
		rectTransform = GetComponent<RectTransform>();
		timeUpdateStep = 1f / FramesPerSecond;
		currentXTileOffset = 0f;
		if (texture == null)
		{
			StartCoroutine(GetWebGifAndSetTexture(gifUrl));
		}
	}

	/// <summary>  
	///  Load animated gif from url, tile frames into a Texture2D.
	/// </summary>  
	IEnumerator GetWebGifAndSetTexture(string url)
	{
		webRequest = new UnityWebRequest(url);
		webRequest.downloadHandler = new DownloadHandlerBuffer();
		yield return webRequest.Send();

		if (webRequest.isError)
		{
			Debug.LogError(webRequest.error);
		}
		else
		{
			imageBuffer = webRequest.downloadHandler.data;
			Image image = Image.FromStream(new MemoryStream(imageBuffer));
			numberOfFrames = image.GetFrameCount(FrameDimension.Time);

			texture = SetupTexture(image.Width, image.Height, numberOfFrames, image);
			InitializeMaterialAndTexture(image.Width, image.Height, numberOfFrames);
		}
	}

	/// <summary>  
	///  Hookup texture to material, set the offsetStep so Update will know how to iterate.
	///  Poke the material so it knows to draw the texture change.
	/// </summary>  
	void InitializeMaterialAndTexture(int width, int height, int numberOfFrames)
	{
		
		rectTransform.sizeDelta = new Vector2(width, height);
		offsetStep = 1f / (float)numberOfFrames;
		foreach(var uiImage in uiImagesToUpdate)
		{
			uiImage.material.SetTexture("_MainTex", texture);
			uiImage.material.SetTextureOffset("_MainTex", new Vector2(0f, 0f));
			uiImage.material.SetTextureScale("_MainTex", new Vector2(offsetStep, 1f));
			uiImage.SetMaterialDirty();
		}
	}

	/// <summary>
	/// Add the UI Image object so that during Update we can let that image know that
	/// its material needs a redraw.
	/// </summary>
	/// <param name="imageToAdd"></param>
	public void AddImageToUpdate(UnityEngine.UI.Image imageToAdd)
	{
		if (!uiImagesToUpdate.Contains(imageToAdd))
		{
			uiImagesToUpdate.Add(imageToAdd);
		}
	}

	/// <summary>  
	///  For every frame of animation, copy into a flattened texture.
	///  TODO - detect when we need to do more sophisticated tiling or we need more than one texture.
	/// </summary>  
	Texture2D SetupTexture(int frameWidth, int frameHeight, int frameCount, Image image)
	{
		Texture2D tex = new Texture2D(frameWidth*frameCount, frameHeight);
		int xOffset = 0;
		for(var frameIndex=0; frameIndex<frameCount; frameIndex++)
		{
			image.SelectActiveFrame(FrameDimension.Time, frameIndex);
			Bitmap bitmap = new Bitmap(image);
			for (var i=0; i<frameWidth; i++)
			{
				for(var j=0; j<frameHeight; j++)
				{
					//convert from System.Drawing.Color (0-255 int values) to UnityEngine.Color (0.0-1.0 values)
					System.Drawing.Color c = bitmap.GetPixel(i, frameHeight - j -1);
					tex.SetPixel(xOffset + i, j, new UnityEngine.Color(c.R / 255f, c.B / 255f, c.G / 255f, c.A / 255f));
				}
			}
			xOffset += frameWidth;
		}
		tex.Apply();
		return tex;
	}

	/// <summary>  
	///  Every frame check if we need to update the frame.
	///  TODO - can probably optimize by using coroutine for update instead of using the Update method.
	/// </summary>  
	void Update () {
		if (numberOfFrames == 0 || uiImagesToUpdate == null) return;

		if (Time.time-timeOfLastUpdate > timeUpdateStep)
		{
			timeOfLastUpdate = Time.time;
			currentXTileOffset += offsetStep;
			if (currentXTileOffset > 1f)
				currentXTileOffset -= 1f;

			foreach (var uiImage in uiImagesToUpdate)
			{
				uiImage.material.SetTextureOffset("_MainTex", new Vector2(currentXTileOffset, 0f));
			}
		}
	}
}


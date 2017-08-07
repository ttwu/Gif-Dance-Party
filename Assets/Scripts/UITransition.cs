using System;
using UnityEngine;

/// <summary>  
/// This class helps control how the main UI element.
/// </summary>  
public class UITransition : MonoBehaviour
{
	private bool isShowing = false;
	[SerializeField]
	private GameObject gifPreviewGO;
	[SerializeField]
	private GifManager gifManager;
	private bool isInitialized = false;

	/// <summary>
	/// Shows or hides the UI elements to browse and add animated gifs to the board.
	/// </summary>
	public void ToggleShowingState()
	{
		isShowing = !isShowing;
		var scaleValue = isShowing ? 1f : 0f;
		//TODO - make this transition prettier
		transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);

		//activate/deactive only as needed so this gif doesn't waste cycles when it's hidden
		gifPreviewGO.SetActive(isShowing);

		if (isShowing && !isInitialized)
		{
			gifManager.InitializePreview();
			isInitialized = true;
		}
	}
}

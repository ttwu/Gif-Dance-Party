using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class provides the animated gif links.  For now it's hardcoded.
/// TODO - Next step to do is to query the Giphy REST Api to return links.
/// Giphy Api Key to use:
///	f16ab1b3a3a145128c6a682eaea92842
/// </summary>
public class GiphyQuery {

	int currentGifIndex = 0;
	int chunkSize = 5;

	static public string[] GetGifUrls(int offset = 0)
	{
		return new string[]{
			"https://media.giphy.com/media/wn8rVP7qC8TNC/giphy.gif",
			"https://media.giphy.com/media/sg32LhHk9RVLi/giphy.gif",
			"https://media.giphy.com/media/nvKjkfWIl2msw/giphy.gif",
			"https://media.giphy.com/media/OqlDrp2Ie0rWE/giphy.gif",
			"https://media.giphy.com/media/bJnqyzhYlF8LS/giphy.gif"
		};
	}

}

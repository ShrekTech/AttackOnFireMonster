using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChoiceHighlighter : MonoBehaviour {	
	public BattleStateHandler battleStateHandler;
	public List<Image> redPlayerIndicators;
	public List<Image> bluePlayerIndicators;
	public List<Image> whitePlayerIndicators;

	// Update is called once per frame
	void Update () {
		foreach(Image redImage in redPlayerIndicators) {
			redImage.color = new Color(
				redImage.color.r, redImage.color.g,
				redImage.color.b, 0.0f);
		}

		foreach(Image blueImage in bluePlayerIndicators) {
			blueImage.color = new Color(
				blueImage.color.r, blueImage.color.g,
				blueImage.color.b, 0.0f);
		}

		foreach(Image whiteImage in whitePlayerIndicators) {
			whiteImage.color = new Color(
				whiteImage.color.r, whiteImage.color.g,
				whiteImage.color.b, 0.0f);
		}

		bool redVoteExists = battleStateHandler.playerVote.ContainsKey ("red");
		bool blueVoteExists = battleStateHandler.playerVote.ContainsKey ("blue");
		bool whiteVoteExists = battleStateHandler.playerVote.ContainsKey ("white");


		if(redVoteExists) {
			int redVote = (int)battleStateHandler.playerVote ["red"];
			if(redVote > 0) {
				//Red vote exists and is not default, so covert into index for indicator array
				redVote -= 1; 
				Image redImage = redPlayerIndicators[redVote];
				redImage.color = new Color(
					redImage.color.r, redImage.color.g,
					redImage.color.b, 1.0f);
			}
		}


		if(blueVoteExists) {
			int blueVote = (int)battleStateHandler.playerVote ["blue"];
			if(blueVote > 0) {
				//Blue vote exists and is not default, so covert into index for indicator array
				blueVote -= 1; 
				Image blueImage = bluePlayerIndicators[blueVote];
				blueImage.color = new Color(
					blueImage.color.r, blueImage.color.g,
					blueImage.color.b, 1.0f);
			}
		}


		if(whiteVoteExists) {
			int whiteVote = (int)battleStateHandler.playerVote ["white"];
			if(whiteVote > 0) {
				//White vote exists and is not default, so covert into index for indicator array
				whiteVote -= 1; 
				Image whiteImage = whitePlayerIndicators[whiteVote];
				whiteImage.color = new Color(
					whiteImage.color.r, whiteImage.color.g,
					whiteImage.color.b, 1.0f);
			}
		}
	}
}

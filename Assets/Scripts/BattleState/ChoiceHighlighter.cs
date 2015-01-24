using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChoiceHighlighter : MonoBehaviour {	
	public BattleStateHandler battleStateHandler;
	public List<Image> redPlayerIndicators;
	public List<Image> bluePlayerIndicators;
	public List<Image> whitePlayerIndicators;
	public List<Image> greenPlayerIndicators;

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

		foreach(Image greenImage in greenPlayerIndicators) {
			greenImage.color = new Color(
				greenImage.color.r, greenImage.color.g,
				greenImage.color.b, 0.0f);
		}

        bool redVoteExists = Voting.ChosenOption[0] != 0;
        bool blueVoteExists = Voting.ChosenOption[1] != 0;
        bool whiteVoteExists = Voting.ChosenOption[2] != 0;
        bool greenVoteExists = Voting.ChosenOption[3] != 0;


		if(redVoteExists) {
            int redVote = Voting.ChosenOption[0];
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
            int blueVote = Voting.ChosenOption[1];
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
            int whiteVote = Voting.ChosenOption[2];
			if(whiteVote > 0) {
				//White vote exists and is not default, so covert into index for indicator array
				whiteVote -= 1; 
				Image whiteImage = whitePlayerIndicators[whiteVote];
				whiteImage.color = new Color(
					whiteImage.color.r, whiteImage.color.g,
					whiteImage.color.b, 1.0f);
			}
		}

		if(greenVoteExists) {
            int greenVote = Voting.ChosenOption[3];
			if(greenVote > 0) {
				//Green vote exists and is not default, so covert into index for indicator array
				greenVote -= 1; 
				Image greenImage = greenPlayerIndicators[greenVote];
				greenImage.color = new Color(
					greenImage.color.r, greenImage.color.g,
					greenImage.color.b, 1.0f);
			}
		}
	}
}

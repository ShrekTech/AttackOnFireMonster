using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BattleScenario {
	public class EndGameState : IBattleState {
		private bool playerWin;
		private bool messageDisplayed = false;

		public EndGameState (bool playerWin)
		{
			this.playerWin = playerWin;
		}

        public IBattleState UpdateState(BattleStateHandler battleStateHandler)
		{
			return this;
		}

        public void Update(BattleStateHandler battleStateHandler)
		{
			if (messageDisplayed) {
				return;
			}
			
			Image endImage;
			if (playerWin){
				endImage = MonoBehaviour.Instantiate (battleStateHandler.winScreen, new Vector2(), Quaternion.identity) as Image;
			} else {
				endImage = MonoBehaviour.Instantiate (battleStateHandler.gameOverScreen, new Vector2(), Quaternion.identity) as Image;
			}
			endImage.transform.SetParent(battleStateHandler.canvas.transform, false);
			messageDisplayed = true;
			return;
		}
	}
}
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
			Text endText = MonoBehaviour.Instantiate (battleStateHandler.endText) as Text;
			endText.transform.SetParent (battleStateHandler.canvas.transform, false);

			if (this.playerWin) {
				endText.text = "YOU WIN!!";
				return;
			}
			endText.text = "TRY AGAIN!!";
			messageDisplayed = true;
		}
	}
}
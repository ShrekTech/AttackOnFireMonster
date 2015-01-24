using UnityEngine;
using System.Collections;

namespace BattleScenario {
	public class CountdownState : IBattleState {

		private const float countdownInitial = 3.0f;
		private float countdown = countdownInitial;
		private static CountdownState INSTANCE;

		public IBattleState UpdateState ()
		{
			if (countdown <= 0) {
				return new PlayerActionState();
			}
			return this;
		}

		public void Update (BattleStateHandler battleStateHandler)
		{
			countdown -= Time.deltaTime;
			Debug.Log ("Countdown: " + countdown);
		}
	}
}
using UnityEngine;
using System.Collections;

namespace BattleScenario {
	public class PlayerActionState : IBattleState {

		private float actionTime = 2.0f;

		public IBattleState UpdateState ()
		{
			if (actionTime <= 0) {
				return new MonsterActionState();
			}
			return this;
		}

		public void Update (BattleStateHandler battleStateHandler)
		{	
			actionTime -= Time.deltaTime;
			Debug.Log ("Player Action");
		}

	}
}
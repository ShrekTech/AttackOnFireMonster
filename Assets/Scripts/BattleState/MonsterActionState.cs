using UnityEngine;
using System.Collections;

namespace BattleScenario
{
	class MonsterActionState : IBattleState
	{
		private float actionTime = 2.0f;

		public IBattleState UpdateState ()
		{
			if (actionTime <= 0) {
				return new CountdownState();
			}
			return this;
		}

		public void Update (BattleStateHandler battleStateHandler)
		{
			actionTime -= Time.deltaTime;
			Debug.Log ("MonsterActionState");
		}
	}

}
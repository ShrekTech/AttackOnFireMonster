using UnityEngine;
using System.Collections;

namespace BattleScenario
{
	class EnemyActionState : IBattleState
	{
		private float actionTime = 2.0f;

        public IBattleState UpdateState(BattleStateHandler battleStateHandler)
		{
			if (actionTime <= 0) {
                Debug.Log("MonsterActionState done");
				return new CountdownState(battleStateHandler);
			}
			return this;
		}

		public void Update (BattleStateHandler battleStateHandler)
		{
			actionTime -= Time.deltaTime;
			
		}
	}

}
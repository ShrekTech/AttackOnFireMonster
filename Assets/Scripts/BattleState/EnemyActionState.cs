using UnityEngine;
using UnityEngine.UI;
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
			if (battleStateHandler.enemy.IsDead()) {
				Color enemyColor = battleStateHandler.enemy.enemyImage.color;
				Color redEnemyColour = new Color (
					1.0f,
					0,
					0,
					enemyColor.a
					);

				battleStateHandler.enemy.enemyImage.color = redEnemyColour;
			}

		}
	}

}
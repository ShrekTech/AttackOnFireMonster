using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BattleScenario
{
	class EnemyActionState : IBattleState
	{
		private float actionTime = 2.0f;
		private bool shotFired = false;

        public IBattleState UpdateState(BattleStateHandler battleStateHandler)
		{
			if (actionTime <= 0) {
				BattleAction enemyAction = new BattleAction(10);
				enemyAction.Apply(battleStateHandler.player);
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
				// maybe should just transition to some game end state?
				actionTime = 0;
				return;
			}

			if (!shotFired) {
				Image fireBall = MonoBehaviour.Instantiate (battleStateHandler.fireballPrefab) as Image;
				fireBall.transform.SetParent(battleStateHandler.canvas.transform, false);
				fireBall.transform.position = battleStateHandler.enemy.transform.position;
				Rigidbody2D fireBallBody = fireBall.GetComponent<Rigidbody2D>();
				fireBallBody.velocity = new Vector2(-1000,-700);
				MonoBehaviour.Destroy(fireBall, 1.0f);
				shotFired = true;
			}
			

		}
	}

}
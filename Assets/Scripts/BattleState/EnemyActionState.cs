using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BattleScenario
{
	class EnemyActionState : IBattleState
	{
		private float actionTime = 1.0f;
        private float initialDelayTime = 1.0f;
		private bool shotFired = false;
		private Image fireBall;

        BattleAction enemyAction;

        public IBattleState UpdateState(BattleStateHandler battleStateHandler)
		{
			if (battleStateHandler.enemy.IsDead()) {
				return new EndGameState(true);
			}

			bool animationDone = shotFired && this.fireBall == null;

			if (animationDone || actionTime <= 0) {
                if (enemyAction != null) {
                    enemyAction.Apply(battleStateHandler.player);
                    battleStateHandler.battleTextField.text = string.Format("JULIANA takes {0} damage!", enemyAction.damage);
                }
				return new CountdownState(battleStateHandler);
			}
			return this;
		}

		public void Update (BattleStateHandler battleStateHandler)
		{
            if (initialDelayTime > 0) {
                initialDelayTime -= Time.deltaTime;
                return;
            }

			actionTime -= Time.deltaTime;

            if (battleStateHandler.currentFireMonsterState == BattleStateHandler.FireMonsterState.PARALYSED) {
                battleStateHandler.battleTextField.text = "The Fire Monster is paralysed!!";
                return;
            }

			if (battleStateHandler.enemy.IsDead()) {
				Color enemyColor = battleStateHandler.enemy.enemyImage.color;
				Color redEnemyColour = new Color (
					1.0f,
					0,
					0,
					enemyColor.a
					);

				battleStateHandler.enemy.enemyImage.color = redEnemyColour;
				return;
			}

			if (!shotFired && battleStateHandler.currentFireMonsterState == BattleStateHandler.FireMonsterState.ATTACKING) {
				this.fireBall = MonoBehaviour.Instantiate (battleStateHandler.enemyFireballPrefab) as Image;
				this.fireBall.transform.SetParent (battleStateHandler.canvas.transform, false);
				shotFired = true;
			}

            // Take action
            enemyAction = new BattleAction(10);
		}
	}

}
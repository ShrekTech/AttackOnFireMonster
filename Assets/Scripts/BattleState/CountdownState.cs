using UnityEngine;
using System.Collections;

namespace BattleScenario {
	public class CountdownState : IBattleState {

		public const float countdownInitial = 3.0f;
        const float syncIntervalSeconds = 1f;
        float nextSync;

        public CountdownState(BattleStateHandler battleStateHandler)
        {
            battleStateHandler.ServerCountdownTime = countdownInitial;

            nextSync = Time.time + syncIntervalSeconds;
            battleStateHandler.SyncCountdownTime();
        }

        public IBattleState UpdateState(BattleStateHandler battleStateHandler)
		{
			if (battleStateHandler.ServerCountdownTime <= 0) {
                Debug.Log("Countdown done");
				return new PlayerActionState();
			}
			return this;
		}

        public void Update(BattleStateHandler battleStateHandler)
		{
            if (Network.isServer) {
                if (Time.time > nextSync) {
                    nextSync = Time.time + syncIntervalSeconds;
                    battleStateHandler.SyncCountdownTime();
                }
            }

            battleStateHandler.ServerCountdownTime -= Time.deltaTime;
		}
	}
}
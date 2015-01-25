using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BattleScenario {
	public class CountdownState : IBattleState {

		public const float countdownInitial = 1.1f;
        const float syncIntervalSeconds = 1f;
        float nextSync;

        bool firstUpdate;

        public CountdownState(BattleStateHandler battleStateHandler)
        {
            battleStateHandler.ServerCountdownTime = countdownInitial;

            nextSync = Time.time + syncIntervalSeconds;
            battleStateHandler.SyncCountdownTime();

            firstUpdate = true;
        }

        public IBattleState UpdateState(BattleStateHandler battleStateHandler)
		{
			if (battleStateHandler.ServerCountdownTime <= 0) {
                Debug.Log("Countdown done");

                battleStateHandler.timerAnimator.Play("TimerOut");

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

            if (firstUpdate) {
                firstUpdate = false;
                battleStateHandler.timerAnimator.Play("TimerIn");
            }

            battleStateHandler.ServerCountdownTime -= Time.deltaTime;

            battleStateHandler.timerDisplay.fillAmount = 1f - (battleStateHandler.ServerCountdownTime / CountdownState.countdownInitial);
		}

        IEnumerator FadeAlpha(CanvasGroup canvasGroup, float target, float duration)
        {
            float startTime = Time.time;
            float startAlpha = canvasGroup.alpha;
            float currentAlpha = startAlpha;
            while (Time.time - startTime <= duration) {
                if (!Mathf.Approximately(canvasGroup.alpha, currentAlpha))
                    break;
                currentAlpha = Mathf.Lerp(startAlpha, target, (Time.time - startTime) / duration);
                canvasGroup.alpha = currentAlpha;
                yield return 0;
            }
            if (Mathf.Approximately(canvasGroup.alpha, currentAlpha))
                canvasGroup.alpha = target;
        }

        IEnumerator Fill(Image image, float target, float duration)
        {
            float startTime = Time.time;
            float start = image.fillAmount;
            float current = start;
            while (Time.time - startTime <= duration) {
                current = Mathf.Lerp(start, target, (Time.time - startTime) / duration);
                image.fillAmount = current;
                yield return 0;
            }
        }
	}
}
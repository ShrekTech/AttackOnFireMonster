using UnityEngine;
using System.Collections;

namespace BattleScenario {
	public interface IBattleState {
		IBattleState UpdateState(BattleStateHandler battleStateHandle);
		void Update(BattleStateHandler battleStateHandler);
	}
}
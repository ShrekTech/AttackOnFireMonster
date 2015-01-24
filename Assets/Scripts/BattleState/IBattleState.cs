using UnityEngine;
using System.Collections;

namespace BattleScenario {
	public interface IBattleState {
		IBattleState UpdateState(BattleStateHandler battleStateHandler);
		void Update(BattleStateHandler battleStateHandler);
	}
}
using UnityEngine;
using System.Collections;

namespace BattleScenario {
	public interface IBattleState {
		IBattleState UpdateState();
		void Update(BattleStateHandler battleStateHandler);
	}
}
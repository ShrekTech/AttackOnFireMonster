using UnityEngine;
using System.Collections;
using BattleScenario;

public class BattleStateHandler : MonoBehaviour {

	private IBattleState currentBattleState;

	void Awake () {
		this.currentBattleState = new CountdownState();
	}

	void Update () {
		this.currentBattleState = this.currentBattleState.UpdateState ();
		this.currentBattleState.Update (this);
	}
}

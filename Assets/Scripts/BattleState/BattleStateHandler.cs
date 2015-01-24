using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BattleScenario;
using UnityEngine.UI;

public class BattleStateHandler : MonoBehaviour {

	private IBattleState currentBattleState;

	public Image fireballPrefab;
	public Image coldballPrefab;

	[System.NonSerialized]
	public Canvas canvas;

	public enum PlayerAction {
		DEFAULT,
		FIREBALL,
		COLDBALL,
		DEFEND
	}

	public PlayerAction[] playerVote = new PlayerAction[Voting.MaxPlayers];

	void Awake () {
		this.canvas = GetComponentInParent<Canvas> ();
		this.currentBattleState = new CountdownState();
	}

	void Update () {
		this.currentBattleState = this.currentBattleState.UpdateState ();
		this.currentBattleState.Update (this);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BattleScenario;
using UnityEngine.UI;

public class BattleStateHandler : MonoBehaviour {

	public enum PlayerAction {
		DEFAULT,
		FIREBALL,
		COLDBALL,
		DEFEND
	}

	private IBattleState currentBattleState;

	public Image fireballPrefab;
	public Image coldballPrefab;

	[System.NonSerialized]
	public Canvas canvas;

	public FireMonster enemy;

	public Dictionary<string, PlayerAction> playerVote = new Dictionary<string, PlayerAction>();

	void Awake () {
		this.canvas = GetComponentInParent<Canvas> ();
		this.currentBattleState = new CountdownState();
	}

	void Update () {
		this.currentBattleState = this.currentBattleState.UpdateState (this);
		this.currentBattleState.Update (this);
	}
}

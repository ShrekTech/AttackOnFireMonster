using UnityEngine;
using System.Collections;

public class PlayerActionVoteCounter : MonoBehaviour {

	public BattleStateHandler battleStateHandler;

	public void Update () {
		//battleStateHandler.playerVote ["MEEEE"] = (BattleStateHandler.PlayerAction) playerActionToVoteFor;
		if(Input.GetKey(KeyCode.A)) {
			battleStateHandler.playerVote["red"] = BattleStateHandler.PlayerAction.FIREBALL;
		}
		if(Input.GetKey(KeyCode.S)) {
			battleStateHandler.playerVote["red"] = BattleStateHandler.PlayerAction.COLDBALL;
		}
		if(Input.GetKey(KeyCode.D)) {
			battleStateHandler.playerVote["red"] = BattleStateHandler.PlayerAction.DEFEND;
		}

		if(Input.GetKey(KeyCode.R)) {
			battleStateHandler.playerVote["blue"] = BattleStateHandler.PlayerAction.FIREBALL;
		}
		if(Input.GetKey(KeyCode.T)) {
			battleStateHandler.playerVote["blue"] = BattleStateHandler.PlayerAction.COLDBALL;
		}
		if(Input.GetKey(KeyCode.Y)) {
			battleStateHandler.playerVote["blue"] = BattleStateHandler.PlayerAction.DEFEND;
		}

		if(Input.GetKey(KeyCode.J)) {
			battleStateHandler.playerVote["white"] = BattleStateHandler.PlayerAction.FIREBALL;
		}
		if(Input.GetKey(KeyCode.K)) {
			battleStateHandler.playerVote["white"] = BattleStateHandler.PlayerAction.COLDBALL;
		}
		if(Input.GetKey(KeyCode.L)) {
			battleStateHandler.playerVote["white"] = BattleStateHandler.PlayerAction.DEFEND;
		}

		if(Input.GetKey(KeyCode.Keypad4)) {
			battleStateHandler.playerVote["green"] = BattleStateHandler.PlayerAction.FIREBALL;
		}
		if(Input.GetKey(KeyCode.Keypad5)) {
			battleStateHandler.playerVote["green"] = BattleStateHandler.PlayerAction.COLDBALL;
		}
		if(Input.GetKey(KeyCode.Keypad6)) {
			battleStateHandler.playerVote["green"] = BattleStateHandler.PlayerAction.DEFEND;
		}
	}
}

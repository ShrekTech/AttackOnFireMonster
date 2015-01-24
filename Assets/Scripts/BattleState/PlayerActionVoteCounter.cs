using UnityEngine;
using System.Collections;

public class PlayerActionVoteCounter : MonoBehaviour {

	public BattleStateHandler battleStateHandler;

	public void Update () {
		if(Input.GetKey(KeyCode.A)) {
			Voting.ChosenOption[Voting.RED] = (int)BattleStateHandler.PlayerAction.FIREBALL;
		}
		if(Input.GetKey(KeyCode.S)) {
            Voting.ChosenOption[Voting.RED] = (int)BattleStateHandler.PlayerAction.COLDBALL;
		}
		if(Input.GetKey(KeyCode.D)) {
            Voting.ChosenOption[Voting.RED] = (int)BattleStateHandler.PlayerAction.DEFEND;
		}

		if(Input.GetKey(KeyCode.R)) {
			Voting.ChosenOption[Voting.BLUE] = (int)BattleStateHandler.PlayerAction.FIREBALL;
		}
		if(Input.GetKey(KeyCode.T)) {
			Voting.ChosenOption[Voting.BLUE] = (int)BattleStateHandler.PlayerAction.COLDBALL;
		}
		if(Input.GetKey(KeyCode.Y)) {
			Voting.ChosenOption[Voting.BLUE] = (int)BattleStateHandler.PlayerAction.DEFEND;
		}

		if(Input.GetKey(KeyCode.J)) {
			Voting.ChosenOption[Voting.WHITE] = (int)BattleStateHandler.PlayerAction.FIREBALL;
		}
		if(Input.GetKey(KeyCode.K)) {
			Voting.ChosenOption[Voting.WHITE] = (int)BattleStateHandler.PlayerAction.COLDBALL;
		}
		if(Input.GetKey(KeyCode.L)) {
			Voting.ChosenOption[Voting.WHITE] = (int)BattleStateHandler.PlayerAction.DEFEND;
		}

		if(Input.GetKey(KeyCode.Keypad4)) {
			Voting.ChosenOption[Voting.GREEN] = (int)BattleStateHandler.PlayerAction.FIREBALL;
		}
		if(Input.GetKey(KeyCode.Keypad5)) {
			Voting.ChosenOption[Voting.GREEN] = (int)BattleStateHandler.PlayerAction.COLDBALL;
		}
		if(Input.GetKey(KeyCode.Keypad6)) {
			Voting.ChosenOption[Voting.GREEN] = (int)BattleStateHandler.PlayerAction.DEFEND;
		}
	}
}

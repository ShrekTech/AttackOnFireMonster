using UnityEngine;
using System.Collections;

public class PlayerActionVoteCounter : MonoBehaviour {

	public void Update () {
		if(Input.GetKeyDown(KeyCode.A)) {
			Voting.ChosenOption[Voting.RED] = (int)BattleStateHandler.PlayerAction.FIREBALL;
		}
        if (Input.GetKeyDown(KeyCode.S)) {
            Voting.ChosenOption[Voting.RED] = (int)BattleStateHandler.PlayerAction.COLDBALL;
		}
        if (Input.GetKeyDown(KeyCode.D)) {
            Voting.ChosenOption[Voting.RED] = (int)BattleStateHandler.PlayerAction.DEFEND;
		}

        if (Input.GetKeyDown(KeyCode.R)) {
			Voting.ChosenOption[Voting.BLUE] = (int)BattleStateHandler.PlayerAction.FIREBALL;
		}
        if (Input.GetKeyDown(KeyCode.T)) {
			Voting.ChosenOption[Voting.BLUE] = (int)BattleStateHandler.PlayerAction.COLDBALL;
		}
        if (Input.GetKeyDown(KeyCode.Y)) {
			Voting.ChosenOption[Voting.BLUE] = (int)BattleStateHandler.PlayerAction.DEFEND;
		}

        if (Input.GetKeyDown(KeyCode.J)) {
			Voting.ChosenOption[Voting.WHITE] = (int)BattleStateHandler.PlayerAction.FIREBALL;
		}
        if (Input.GetKeyDown(KeyCode.K)) {
			Voting.ChosenOption[Voting.WHITE] = (int)BattleStateHandler.PlayerAction.COLDBALL;
		}
        if (Input.GetKeyDown(KeyCode.L)) {
			Voting.ChosenOption[Voting.WHITE] = (int)BattleStateHandler.PlayerAction.DEFEND;
		}

        if (Input.GetKeyDown(KeyCode.Keypad4)) {
			Voting.ChosenOption[Voting.GREEN] = (int)BattleStateHandler.PlayerAction.FIREBALL;
		}
        if (Input.GetKeyDown(KeyCode.Keypad5)) {
			Voting.ChosenOption[Voting.GREEN] = (int)BattleStateHandler.PlayerAction.COLDBALL;
		}
        if (Input.GetKeyDown(KeyCode.Keypad6)) {
			Voting.ChosenOption[Voting.GREEN] = (int)BattleStateHandler.PlayerAction.DEFEND;
		}
	}
}

using UnityEngine;
using System.Collections;

public class PlayerActionVoteCounter : MonoBehaviour {

	public BattleStateHandler battleStateHandler;

	public void OnClick (int playerActionToVoteFor) {
        battleStateHandler.playerVote[Voting.GlobalClientIndex] = (BattleStateHandler.PlayerAction)playerActionToVoteFor;
	}
}

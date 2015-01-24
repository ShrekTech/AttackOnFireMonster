using UnityEngine;
using System.Collections;

public class PlayerActionVoteCounter : MonoBehaviour {

	public BattleStateHandler battleStateHandler;

	public void OnClick (int playerActionToVoteFor) {
		battleStateHandler.playerVote ["MEEEE"] = (BattleStateHandler.PlayerAction) playerActionToVoteFor;
	}
}

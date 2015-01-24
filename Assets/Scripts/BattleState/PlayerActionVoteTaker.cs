using UnityEngine;
using System.Collections;

public class PlayerActionVoteTaker : MonoBehaviour {
    public Voting PlayerZeroVoter;

	public void OnClick (int playerActionToVoteFor) {
        PlayerZeroVoter.SendOption(playerActionToVoteFor);
	}
}

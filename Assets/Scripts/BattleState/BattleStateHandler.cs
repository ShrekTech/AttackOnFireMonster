using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BattleScenario;
using UnityEngine.UI;

public class BattleStateHandler : MonoBehaviour {

	private IBattleState currentBattleState;

	public Image fireballPrefab;
	public Image coldballPrefab;

    public Image timerDisplay;

	[System.NonSerialized]
	public Canvas canvas;

	public FireMonster enemy;
    public float ServerCountdownTime;

    public int highestVotedAction;

    [RPC]
    public void SetHighestVotedAction(int highestVotedAction)
    {
        this.highestVotedAction = highestVotedAction;
    }

    [RPC]
    void SetCountdownTime(float time)
    {
        ServerCountdownTime = time;
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        networkView.RPC("SetCountdownTime", player, ServerCountdownTime);
    }

    public void SyncCountdownTime()
    {
        for (int playerIndex = 0; playerIndex < Network.connections.Length; ++playerIndex) {
            var player = Network.connections[playerIndex];
            int pingMilisec = Network.GetAveragePing(player);
            float pingSec = pingMilisec * (1.0f / 1000f);
            networkView.RPC("SetCountdownTime", player, ServerCountdownTime + pingSec);
        }
    }

    public enum PlayerAction : int
    {
		DEFAULT,
		FIREBALL,
		COLDBALL,
		DEFEND
	}

	void Awake () {
		this.canvas = GetComponentInParent<Canvas> ();
		this.currentBattleState = new CountdownState(this);

        //timerDisplay.color = Color.clear;
	}

	void Update () {
        this.currentBattleState = this.currentBattleState.UpdateState(this);
		this.currentBattleState.Update (this);
	}
}

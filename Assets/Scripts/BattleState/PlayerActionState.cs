using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BattleScenario {
	public class PlayerActionState : IBattleState {

		private float actionTime = 2.0f;
		private bool majorityDecision = false;
		private bool actionPerformed = false;

        public IBattleState UpdateState(BattleStateHandler battleStateHandler)
		{
			if (actionTime <= 0) {
				return new MonsterActionState();
			}
			return this;
		}

		public void Update(BattleStateHandler battleStateHandler)
		{
            actionTime -= Time.deltaTime;

            if (actionPerformed) {
                return;
            }

            if (Network.isServer) {
                var playerVotes = Voting.ChosenOption;

                int highestVotedAction = 0;

                if (battleStateHandler.highestVotedAction == 0) {
                    highestVotedAction = TallyVotes(playerVotes);
                    System.Array.Clear(playerVotes, 0, playerVotes.Length);
                }

                if (!majorityDecision) {
                    //blow up
                    return;
                }

                battleStateHandler.networkView.RPC("SetWinner", RPCMode.All, highestVotedAction);
            }

            if (battleStateHandler.highestVotedAction != 0) {

                switch ((BattleStateHandler.PlayerAction)battleStateHandler.highestVotedAction) {
                    case BattleStateHandler.PlayerAction.FIREBALL: {
                            Image fireBall = Object.Instantiate(battleStateHandler.fireballPrefab, new Vector2(), Quaternion.identity) as Image;
                            fireBall.transform.SetParent(battleStateHandler.canvas.transform, false);
                            Vector3 shiftedPosition = fireBall.transform.position;
                            shiftedPosition.x += 100;
                            shiftedPosition.y += 100;
                            fireBall.transform.position = shiftedPosition;
                            Object.Destroy(fireBall, 2.0f);
                        } break;
                    case BattleStateHandler.PlayerAction.COLDBALL: {
                            Image coldBall = Object.Instantiate(battleStateHandler.coldballPrefab, new Vector2(), Quaternion.identity) as Image;
                            coldBall.transform.SetParent(battleStateHandler.canvas.transform, false);
                            Vector3 shiftedPosition = coldBall.transform.position;
                            shiftedPosition.x += 100;
                            shiftedPosition.y += 100;
                            coldBall.transform.position = shiftedPosition;
                            Object.Destroy(coldBall, 2.0f);
                        } break;
                    case BattleStateHandler.PlayerAction.DEFEND:
                        break;
                    case BattleStateHandler.PlayerAction.DEFAULT:
                        break;
                    default:
                        Debug.LogError(string.Format("Unhandled player action '{0}'... won somehow?.", battleStateHandler.highestVotedAction));
                        break;
                }

                battleStateHandler.highestVotedAction = 0;
                actionPerformed = true;
            }
		}

        int TallyVotes(int[] playerVotes)
		{
            List<Votes> talliedVotes = new List<Votes>();
            Votes fireballVotes = new Votes(BattleStateHandler.PlayerAction.FIREBALL);
            Votes iceballVotes = new Votes(BattleStateHandler.PlayerAction.COLDBALL);
            Votes defaultVotes = new Votes(BattleStateHandler.PlayerAction.DEFEND);
            talliedVotes.Add(fireballVotes);
            talliedVotes.Add(iceballVotes);
            talliedVotes.Add(defaultVotes);
            for (int playerIndex = 0; playerIndex < playerVotes.Length; ++playerIndex) {
                switch ((BattleStateHandler.PlayerAction)playerVotes[playerIndex]) {
                    case BattleStateHandler.PlayerAction.FIREBALL:
                        fireballVotes.IncrementVoteCount();
                        break;
                    case BattleStateHandler.PlayerAction.COLDBALL:
                        iceballVotes.IncrementVoteCount();
                        break;
                    case BattleStateHandler.PlayerAction.DEFEND:
                        defaultVotes.IncrementVoteCount();
                        break;
                    case BattleStateHandler.PlayerAction.DEFAULT:
                        break;
                    default:
                        Debug.LogError(string.Format("Unhandled player action '{0}' got votes.", playerVotes[playerIndex]));
                        break;
                }
            }
			talliedVotes.Sort ();
            int highestVotedAction = (int)talliedVotes[0].GetPlayerAction();

            if (talliedVotes[0].GetVoteCount() == talliedVotes[1].GetVoteCount()) {
                // indecision
                majorityDecision = false;
            }
            else {
                majorityDecision = true;
            }

            return highestVotedAction;
		}

		public class Votes : System.IComparable<Votes>
		{
			private BattleStateHandler.PlayerAction playerAction;
			private int votes;

			public Votes(BattleStateHandler.PlayerAction playerAction) {
				this.playerAction = playerAction;
				this.votes = 0;
			}

			public BattleStateHandler.PlayerAction GetPlayerAction ()
			{
				return this.playerAction;
			}

			public int GetVoteCount ()
			{
				return this.votes;
			}

			public void IncrementVoteCount ()
			{
				++this.votes;
			}

            public int CompareTo(Votes other)
            {
				return other.votes - this.votes;
			}
		}
	}
}
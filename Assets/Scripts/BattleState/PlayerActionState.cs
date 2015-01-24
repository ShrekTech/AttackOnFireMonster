using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BattleScenario {
	public class PlayerActionState : IBattleState {

		private float actionTime = 2.0f;
		private bool majorityDecision = false;
		private bool actionPerformed = false;
		private Image attackBallImage;
		public BattleAction playerAction;

        public IBattleState UpdateState(BattleStateHandler battleStateHandler)
		{
			if (actionTime <= 0) {
				if(playerAction != null) {
					if(playerAction.GetTarget() == BattleAction.Target.Self) {
						// TODO: apply action to player
					} else if(playerAction.GetTarget() == BattleAction.Target.Enemy) {
						playerAction.Apply(battleStateHandler.enemy);
					}
				}

				return new EnemyActionState();
			}
			return this;
		}

        public void Update(BattleStateHandler battleStateHandler)
        {
            actionTime -= Time.deltaTime;

            AnimateAttackBall(Time.deltaTime);

            if (actionPerformed) {
                return;
            }

            if (Network.isServer) {
                int highestVotedAction = 0;

                if (battleStateHandler.highestVotedAction == 0) {
                    highestVotedAction = TallyVotes(Voting.ChosenOption);

                    battleStateHandler.networkView.RPC("FinaliseHighestVotedAction", RPCMode.All, highestVotedAction);
                }
            }

            if (battleStateHandler.highestVotedAction != 0) {
                switch ((BattleStateHandler.PlayerAction)battleStateHandler.highestVotedAction) {
                    case BattleStateHandler.PlayerAction.FIREBALL: {
                            playerAction = new BattleAction(45, BattleAction.DamageType.Fire);
                            attackBallImage = MonoBehaviour.Instantiate(battleStateHandler.fireballPrefab, new Vector2(), Quaternion.identity) as Image;
                            attackBallImage.transform.SetParent(battleStateHandler.canvas.transform, false);
                            MonoBehaviour.Destroy(attackBallImage, 2.0f);
                        }
                        break;
                    case BattleStateHandler.PlayerAction.COLDBALL: {
                            playerAction = new BattleAction(45, BattleAction.DamageType.Cold);
                            attackBallImage = MonoBehaviour.Instantiate(battleStateHandler.coldballPrefab, new Vector2(), Quaternion.identity) as Image;
                            attackBallImage.transform.SetParent(battleStateHandler.canvas.transform, false);
                            MonoBehaviour.Destroy(attackBallImage, 2.0f);
                            break;
                        }
                    case BattleStateHandler.PlayerAction.DEFEND:
                        playerAction = new BattleAction(-20, BattleAction.DamageType.RegularType, BattleAction.Target.Self);
                        break;
                    default:
                        Debug.LogError("Unhandled player action");
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

		void AnimateAttackBall(float deltaTime) {
			if (attackBallImage != null) {
				Vector3 shiftedPosition = attackBallImage.transform.position;
				// TODO: probably don't hard code these
				shiftedPosition.x += 100 * deltaTime;
				shiftedPosition.y += 100 * deltaTime;
				attackBallImage	.transform.position = shiftedPosition;
			}
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
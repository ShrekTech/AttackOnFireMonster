using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BattleScenario {
	public class PlayerActionState : IBattleState {

		private float actionTimeout = 5.0f;
		private bool actionPerformed = false;
		private Image attackBallImage;
		public BattleAction playerAction;

        public IBattleState UpdateState(BattleStateHandler battleStateHandler)
		{
			if (battleStateHandler.player.IsDead ()) {
				return new EndGameState(false);
			}

            bool animationDone = actionPerformed && attackBallImage == null;
            if (animationDone || actionTimeout <= 0) {
                if (playerAction != null) {

                    if (playerAction.GetTarget() == BattleAction.Target.Self) {
						if(playerAction.damage < 0) {
							battleStateHandler.battleTextField.text = string.Format("HEAL {0} Hit Points!", -playerAction.damage);
						} else {
							battleStateHandler.battleTextField.text = string.Format("JULIANA hurt herself in her confusion! Self inflicted {0} Hit Points.", playerAction.damage);
						}
						playerAction.Apply(battleStateHandler.player);
                    }
                    else if (playerAction.GetTarget() == BattleAction.Target.Enemy) {
						if (playerAction.type != BattleAction.DamageType.Failure) {

							playerAction.Apply(battleStateHandler.enemy);

							string messageText = string.Format("{0} takes {1} {2} damage!", battleStateHandler.enemy.name, playerAction.damage, playerAction.type);

							if((playerAction.type == BattleAction.DamageType.Fire) && (battleStateHandler.currentFireMonsterState == BattleStateHandler.FireMonsterState.ATTACKING)) {

								if(battleStateHandler.numberOfFireBallsHitBy < 1 ) {
									messageText += "!  Fire Monster is uncomfortably hot!";
									++ battleStateHandler.numberOfFireBallsHitBy;
								} else {
									messageText += "!  The Fire Monster is paralysed!!";
									battleStateHandler.numberOfFireBallsHitBy = 0;
									battleStateHandler.currentFireMonsterState = BattleStateHandler.FireMonsterState.PARALYSED;
								}
							} else if (battleStateHandler.currentFireMonsterState == BattleStateHandler.FireMonsterState.PARALYSED) {
								messageText += "!  Paralysis undone!!";
								battleStateHandler.numberOfFireBallsHitBy = 0;
								battleStateHandler.currentFireMonsterState = BattleStateHandler.FireMonsterState.ATTACKING;
							}

							battleStateHandler.battleTextField.text = messageText;
						
						}
						else {
							battleStateHandler.battleTextField.text = string.Format("{0} tried to {1} but it failed!", "JULIANA", "DEMOCRACY", playerAction.type);
						}
                    }
                }

				return new EnemyActionState();
			}
			return this;
		}

        public void Update(BattleStateHandler battleStateHandler)
        {
            actionTimeout -= Time.deltaTime;

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
					case BattleStateHandler.PlayerAction.FIREBALL:
							{
								playerAction = new BattleAction (0, BattleAction.DamageType.Fire);
								attackBallImage = Object.Instantiate (battleStateHandler.fireballPrefab, new Vector2 (), Quaternion.identity) as Image;
								attackBallImage.transform.SetParent (battleStateHandler.canvas.transform, false);
								break;
							}
					case BattleStateHandler.PlayerAction.COLDBALL:
							{
								playerAction = new BattleAction (12, BattleAction.DamageType.Cold);
								attackBallImage = Object.Instantiate (battleStateHandler.coldballPrefab, new Vector2 (), Quaternion.identity) as Image;
								attackBallImage.transform.SetParent (battleStateHandler.canvas.transform, false);
								break;
							}
					case BattleStateHandler.PlayerAction.DEFEND:
							{
								playerAction = new BattleAction (-11, BattleAction.DamageType.RegularType, BattleAction.Target.Self);
								attackBallImage = Object.Instantiate (battleStateHandler.healBallPrefab, new Vector2 (), Quaternion.identity) as Image;
								attackBallImage.transform.SetParent (battleStateHandler.canvas.transform, false);
								break;
							}
					default:
							Debug.LogError ("Unhandled player action");
							break;
					}
					battleStateHandler.highestVotedAction = 0;
					actionPerformed = true;
			} else {
				playerAction = new BattleAction (5, BattleAction.DamageType.RegularType, BattleAction.Target.Self);
				attackBallImage = Object.Instantiate (battleStateHandler.selfHitBallPrefab, new Vector2 (), Quaternion.identity) as Image;
				attackBallImage.transform.SetParent (battleStateHandler.canvas.transform, false);
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
				// indecision. '0' represents both default and indecision for now
				return 0;
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
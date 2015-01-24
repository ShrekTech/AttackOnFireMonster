using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BattleScenario {
	public class PlayerActionState : IBattleState {

		private float actionTime = 2.0f;
		private Boolean majorityDecision = false;
		private Boolean actionPerformed = false;
		private BattleStateHandler.PlayerAction highestVotedAction = BattleStateHandler.PlayerAction.DEFAULT;
		private Image attackBallImage;
		public BattleAction playerAction;

		public IBattleState UpdateState (BattleStateHandler battleStateHandle)
		{
			if (actionTime <= 0) {
				if(playerAction != null) {
					if(playerAction.GetTarget() == BattleAction.Target.Self) {
						// TODO: apply action to player
					} else if(playerAction.GetTarget() == BattleAction.Target.Enemy) {
						// TODO: add this back in when enemies exist
						//playerAction.Apply(battleStateHandle.enemy);
					}
				}

				return new EnemyActionState();
			}
			return this;
		}

		public void Update (BattleStateHandler battleStateHandler)
		{	
			actionTime -= Time.deltaTime;

			AnimateAttackBall (Time.deltaTime);

			if (actionPerformed) {
				return;
			}

			var playerVotes = battleStateHandler.playerVote;

			if (this.highestVotedAction.Equals(BattleStateHandler.PlayerAction.DEFAULT)) {
				TallyVotes (playerVotes);
			}

			if (!majorityDecision) {
				//blow up
				return;
			}

			switch (this.highestVotedAction) {
				case BattleStateHandler.PlayerAction.FIREBALL:
					{
						playerAction = new BattleAction(45, BattleAction.DamageType.Fire);
						attackBallImage = MonoBehaviour.Instantiate (battleStateHandler.fireballPrefab, new Vector2 (), Quaternion.identity) as Image;
							attackBallImage.transform.SetParent (battleStateHandler.canvas.transform, false);
							MonoBehaviour.Destroy(attackBallImage, 2.0f);
					}
					break;
				case BattleStateHandler.PlayerAction.COLDBALL:
					{
							playerAction = new BattleAction(45, BattleAction.DamageType.Cold);
							attackBallImage = MonoBehaviour.Instantiate (battleStateHandler.coldballPrefab, new Vector2 (), Quaternion.identity) as Image;
							attackBallImage.transform.SetParent (battleStateHandler.canvas.transform, false);
							MonoBehaviour.Destroy(attackBallImage, 2.0f);
							break;
					}
				case BattleStateHandler.PlayerAction.DEFEND:
					break;
				default:
					throw new ArgumentOutOfRangeException ();
			}
			actionPerformed = true;
		}

		void TallyVotes (Dictionary<string, BattleStateHandler.PlayerAction> playerVotes)
		{
			List<Votes> talliedVotes = new List<Votes> ();
			Votes fireballVotes = new Votes (BattleStateHandler.PlayerAction.FIREBALL);
			Votes iceballVotes = new Votes (BattleStateHandler.PlayerAction.COLDBALL);
			Votes defaultVotes = new Votes (BattleStateHandler.PlayerAction.DEFEND);
			talliedVotes.Add (fireballVotes);
			talliedVotes.Add (iceballVotes);
			talliedVotes.Add (defaultVotes);
			foreach (var playerVote in playerVotes) {
				switch (playerVote.Value) {
				case BattleStateHandler.PlayerAction.FIREBALL:
					fireballVotes.IncrementVoteCount ();
					break;
				case BattleStateHandler.PlayerAction.COLDBALL:
					iceballVotes.IncrementVoteCount ();
					break;
				case BattleStateHandler.PlayerAction.DEFEND:
					defaultVotes.IncrementVoteCount ();
					break;
				default:
					throw new ArgumentOutOfRangeException ();
				}
			}
			talliedVotes.Sort ();
			this.highestVotedAction = talliedVotes [0].GetPlayerAction ();

			if (talliedVotes [0].GetVoteCount () == talliedVotes [1].GetVoteCount ()) {
				// indecision
				majorityDecision = false;
			} else {
				majorityDecision = true;
			}
			playerVotes.Clear ();
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

		public class Votes : IComparable<Votes>
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

			public int CompareTo (Votes other) {
				return other.votes - this.votes;
			}
		}
	}
}
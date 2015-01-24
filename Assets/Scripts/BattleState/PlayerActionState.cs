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

		public IBattleState UpdateState ()
		{
			if (actionTime <= 0) {
				return new MonsterActionState();
			}
			return this;
		}

		public void Update (BattleStateHandler battleStateHandler)
		{	
			actionTime -= Time.deltaTime;

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
						Image fireBall = MonoBehaviour.Instantiate (battleStateHandler.fireballPrefab, new Vector2 (), Quaternion.identity) as Image;
							fireBall.transform.SetParent (battleStateHandler.canvas.transform, false);
							Vector3 shiftedPosition = fireBall.transform.position;
							shiftedPosition.x += 200;
							shiftedPosition.y += 200;
							fireBall.transform.position = shiftedPosition;
							MonoBehaviour.Destroy(fireBall, 2.0f);
					}
					break;
				case BattleStateHandler.PlayerAction.COLDBALL:
					{
							Image coldBall = MonoBehaviour.Instantiate (battleStateHandler.coldballPrefab, new Vector2 (), Quaternion.identity) as Image;
							coldBall.transform.SetParent (battleStateHandler.canvas.transform, false);
							Vector3 shiftedPosition = coldBall.transform.position;
							shiftedPosition.x += 200;
							shiftedPosition.y += 200;
							coldBall.transform.position = shiftedPosition;	
							MonoBehaviour.Destroy(coldBall, 2.0f);
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
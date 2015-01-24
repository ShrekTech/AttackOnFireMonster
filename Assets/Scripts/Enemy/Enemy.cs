using System.Collections.Generic;
using UnityEngine;

namespace ai {
	public class Enemy {
		private List<string> actions;
		private string name;
		
		public Enemy(string name, List<string> actions) {
			this.name = name;
			this.actions = actions;
		}
		
		public string SelectBattleAction() {
			return actions[Random.Range(0, actions.Count)];
		}
	}
}
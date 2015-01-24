using System;
using System.Coleections.Generic;
using Unity;

namespace ai {
	public class Enemy {
		private List<BattleAction> actions;
		private string name;
		
		Enemy(string name, List<BattleAction> actions) {
			this.name = name;
			this.actions = actions;
		}
		
		public BattleAction SelectBattleAction() {
			return actions[Random.Range(0, actions.count)];
		}
	}
}
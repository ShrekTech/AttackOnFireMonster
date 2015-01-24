using System;
using System.Coleections.Generic;

namespace ai {
	public class Enemy {
		private List<BattleAction> actions;
		private string name;
		
		public BattleAction SelectBattleAction() {
			Random r = new Random();
			
			int element = r.next(actions.count);
			
			return actions[element];
		}
	}
}
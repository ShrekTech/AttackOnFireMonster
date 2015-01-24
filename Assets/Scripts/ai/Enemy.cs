namespace ai {
	public class Enemy {
		private List<BattleAction> actions;
		private string name;
		
		public BattleAction SelectBattleAction() {
			var r = new Random();
			
			int element = r.next (actions.count);
			
			return actions[element];
		}
	}
}
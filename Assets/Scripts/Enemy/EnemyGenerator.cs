using Unity;
using System.Collections.Generic;

namespace ai {
	public class EnemyGenerator {
		private List<Enemy> enemyPool = new List<Enemy>();
		
		public EnemyGenerator() {
			this.ememyPool = {
				new Enemy("Fire Monster", {"Fireball", "Cold ball"}),
				new Enemy("Sound Designer", {"Fail to exist"})
			};
		}
		
		public Enemy GenerateEnemy() {
			return enemyPool[Random.Range(0, this.enemyPool.count)]
		}
	}
}
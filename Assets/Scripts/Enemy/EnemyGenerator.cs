using UnityEngine;
using System.Collections.Generic;

namespace ai {
	public class EnemyGenerator {
		private List<Enemy> enemyPool = new List<Enemy>();
		
		public EnemyGenerator() {
			this.enemyPool = new List<Enemy> (){
				new Enemy("Fire Monster", new List<string>(){"Fireball", "Cold ball"}),
				new Enemy("Sound Designer", new List<string>(){"Fail to exist"}),
				new Enemy("Man with pitchfork", new List<string>(){"Throw potatoes", "Pitchfork"})
			};
		}
		
		public Enemy GenerateEnemy() {
			return enemyPool[(int) (Random.Range(0f, (float) this.enemyPool.Count))];
		}
	}
}
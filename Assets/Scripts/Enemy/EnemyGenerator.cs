using UnityEngine;
using System.Collections.Generic;

public class EnemyGenerator {
	private List<Enemy> enemyPool = new List<Enemy>();
	
	public EnemyGenerator() {
		enemyPool = new List<Enemy>();
		//enemyPool.Add(new Enemy("Fire Monster", {"Fireball", "Cold ball"}));
		//enemyPool.Add(new Enemy("Sound Designer", {"Fail to exist"}));
		//enemyPool.Add(new Enemy("Man with pitchfork", {"Throw potatoes", "Pitchfork"}));
	}
	
	public Enemy GenerateEnemy() {
		return enemyPool[Random.Range(0, this.enemyPool.Count)];
	}
}
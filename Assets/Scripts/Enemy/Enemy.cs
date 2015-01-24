using UnityEngine;
using System.Collections.Generic;


public abstract class Enemy : MonoBehaviour {
	private int maxHitPoints;
	private int currentHitPoints;
	private List<BattleAction> actions;

	
	public Enemy(string name, int maxHitPoints, List<BattleAction> actions) {
		this.name = name;
		this.maxHitPoints = maxHitPoints;
		this.currentHitPoints = this.maxHitPoints;
		this.actions = actions;
	}
	
	public BattleAction SelectBattleAction() {
		return actions[Random.Range(0, actions.Count)];
	}

	public void TakeDamage(int damage) {
		this.currentHitPoints -= damage;
	}

	public bool IsDead() {
		return currentHitPoints <= 0;
	}
}

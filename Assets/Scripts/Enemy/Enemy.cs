using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public abstract class Enemy : MonoBehaviour {
	public int maxHitPoints;
    public int currentHitPoints;
	public List<BattleAction> actions;

	public Image enemyImage;
	
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

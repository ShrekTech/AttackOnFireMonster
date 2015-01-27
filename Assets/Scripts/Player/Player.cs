using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IBattleActionable {
	
	public string name = "JULIANA";
	public int maxHitPoints;
	public int currentHitPoints;

	void Awake()
	{
		currentHitPoints = this.maxHitPoints;
	}
	
	public System.Collections.Generic.List<BattleAction.DamageType> getResists ()
	{
		return new System.Collections.Generic.List<BattleAction.DamageType> ();
	}

	public void TakeDamage (int damage, BattleAction.DamageType type)
	{
		this.currentHitPoints -= damage;
		Debug.Log ("Player health: " + this.currentHitPoints);
	}

	public bool IsDead() {
		return currentHitPoints <= 0;
	}
}

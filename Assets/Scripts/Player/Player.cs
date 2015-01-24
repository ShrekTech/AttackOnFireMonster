using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IBattleActionable {
	
	public int maxHitPoints;
	private int currentHitPoints;

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
}

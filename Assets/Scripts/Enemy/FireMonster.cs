using UnityEngine;
using System.Collections.Generic;

public class FireMonster : Enemy, IBattleActionable {

	public const string NAME = "Fire Monster";
	public const int MAX_HITPOINTS = 100;

    void Awake()
    {
        name = NAME;
        maxHitPoints = MAX_HITPOINTS;
        currentHitPoints = MAX_HITPOINTS;
    }

	public List<BattleAction.DamageType> getResists ()
	{
		return new List<BattleAction.DamageType>();
	}

	public void TakeDamage (int damage, BattleAction.DamageType type)
	{
		base.TakeDamage(damage);
	}
}

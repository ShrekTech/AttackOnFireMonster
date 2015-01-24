using UnityEngine;
using System.Collections.Generic;

public class FireMonster : Enemy, IBattleActionable {

	public const string NAME = "Fire Monster";
	public const int MAX_HITPOINTS = 100;

	public FireMonster() : base(NAME, MAX_HITPOINTS, new List<BattleAction>()) {
		// Add some battle actions
	}

	#region IBattleActionable implementation

	public List<BattleAction.DamageType> getResists ()
	{
		return new List<BattleAction.DamageType>();
	}

	public void TakeDamage (int damage, BattleAction.DamageType type)
	{
		base.TakeDamage(damage);
	}

	#endregion
}

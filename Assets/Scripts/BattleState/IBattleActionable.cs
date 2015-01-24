using System.Collections.Generic;

public interface IBattleActionable {
	List<BattleAction.DamageType> getResists();
	void TakeDamage(int damage, BattleAction.DamageType type);
}

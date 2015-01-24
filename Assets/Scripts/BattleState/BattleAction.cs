[System.Serializable]
public class BattleAction {
	public enum DamageType {RegularType, Fire, Cold};
	public enum Target {Self, Enemy};

	public int damage;
	public DamageType type;
	public Target target;

	public BattleAction(int damage) {
		this.damage = damage;
		this.type = DamageType.RegularType;
		this.target = Target.Enemy;
	}

	public BattleAction(int damage, DamageType type) {
		this.damage = damage;
		this.type = type;
		this.target = Target.Enemy;
	}

	public BattleAction(int damage, Target target) {
		this.damage = damage;
		this.type = DamageType.RegularType;
		this.target = target;
	}

	public BattleAction(int damage, DamageType type, Target target) {
		this.damage = damage;
		this.type = type;
		this.target = target;
	}

	public void Apply(IBattleActionable recipient) {
		recipient.TakeDamage (damage, type);
	}

	public Target GetTarget() {
		return target;
	}
}

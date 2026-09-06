
public enum Team { Player, Enemy };

public enum Position { front,mid,back};


public  enum ActionType { Attack,Support,disturb}

public  enum ActionDescription {Deal,Heal,Buff,Debuff }

public enum BuffType {none, HPBoost,ATKBoost,DamageCut,
    SpeedBoost
}
public enum DebuffType
{
    none, ATKHit, DamageSuppement,Stun, forced, SpeedHit, focused
}

public enum AttackType { single,Circlerange}

public enum Priority { Distance,HP,self,Special};

public enum PrioritySort { highest,secondHighest,Lowest,SecondLowest,RandomAndNotMultiple, RandomAndMultiple, All};

public enum Shopid { Daily }

public enum charactersortoption1 { LV, name,position}
public enum charactersortoption2 { Desc, Asc }

//public enum SpawnType {Round,Timer }
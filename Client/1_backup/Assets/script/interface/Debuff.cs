using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff: StatusEffect
{
    public DebuffType type;
}

public class focused : Debuff
{

    public focused(GameCharacter Target, float figure, float time)
    {
        this.Target = Target;
        this.figure = figure;
        this.time = time;
        StatusCode = "Substiude";
        type = DebuffType.focused;
    }
    protected override void BuffActive()
    {
        Target.focused = true;
    }
    protected override void BuffDeactive()
    {
        Target.focused = false;
        base.BuffDeactive();
    }
}

public class Stun : Debuff
{
    public Stun(GameCharacter Target, float figure, float time)
    {
        this.Target = Target;
        this.time = time;
        StatusCode = "Stun";
        type = DebuffType.Stun;
    }
    protected override void BuffActive()
    {
        Target.set_cant_handle_time(time);
    }
    protected override void BuffDeactive()
    {
        Target.set_cant_handle_time(0);
        base.BuffDeactive();
    }
}
public class DamageSuppement : Debuff
{
    public DamageSuppement(GameCharacter Target, float figure, float time)
    {
        this.Target = Target;
        this.figure = figure;
        this.time = time;
        StatusCode = "Damagesupplement";
        type = DebuffType.DamageSuppement;
    }
    protected override void BuffActive()
    {
        float tmp = 1 * ((figure + 100) / 100);
        Target.HitteDamageNumber = tmp > Target.HitteDamageNumber ? tmp : Target.HitteDamageNumber;
    }
    protected override void BuffDeactive()
    {
        Target.HitteDamageNumber = 1;
        base.BuffDeactive();
    }
}
public class ATKHit : Debuff
{
    public ATKHit(GameCharacter Target, float figure, float time)
    {
        this.Target = Target;
        this.figure = figure;
        this.time = time;
        StatusCode = "ATKHit";
        type = DebuffType.ATKHit;
    }
    protected override void BuffActive()
    {
        int atk = Target.ATK - Mathf.RoundToInt(Target.ATK * figure);
        Target.current_atk = atk > Target.current_atk ? atk : Target.current_atk;
    }
    protected override void BuffDeactive()
    {
        Target.current_atk = Target.ATK;
        base.BuffDeactive();
    }
}
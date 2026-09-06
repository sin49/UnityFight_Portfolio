using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StatusEffect
{
    public String StatusCode;
    public float figure;
    public float time;
    public GameCharacter Target;
    protected virtual void BuffActive() { }
    protected virtual void BuffDeactive()
    {
        Target.StautusEffectList.Remove(this);
    }
    public bool invoke()
    {
        time -= Time.deltaTime;
        BuffActive();
        if (time > 0)
            return false;
        else
        {
            BuffDeactive();
            return true;
        }
    }
}
public class Buff : StatusEffect
{

    public BuffType Type;

}
public class ATKBoost : Buff
{
  public ATKBoost(GameCharacter Target,float figure,float time)
    {
        this.Target = Target;
        this.figure = figure;
        this.time = time;
        StatusCode = "ATKBoost";
        Type = BuffType.ATKBoost;
    }
    protected override void BuffActive()
    {
        int atk = Target.ATK + Mathf.RoundToInt(Target.ATK * (figure / 100));
        Target.current_atk = atk>Target.current_atk?atk:Target.current_atk;
    }
    protected override void BuffDeactive()
    {
        Target.current_atk = Target.ATK;
        base.BuffDeactive();
    }
}


public class DamageCut : Buff
{

    public DamageCut(GameCharacter Target, float figure, float time)
    {
        this.Target = Target;
        this.figure = figure;
        this.time = time;
        StatusCode = "DamageCut";
        Type = BuffType.DamageCut;
    }
    protected override void BuffActive()
    {
        float tmp = 1 * (figure  / 100);
        Target.HitteDamageNumber = tmp > Target.HitteDamageNumber ? tmp : Target.HitteDamageNumber;
    }
    protected override void BuffDeactive()
    {
        Target.HitteDamageNumber = 1;
        base.BuffDeactive();
    }
}


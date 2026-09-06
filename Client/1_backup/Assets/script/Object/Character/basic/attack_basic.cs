using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack_basic : iAct
{
  
   protected GameCharacter _target;
    //chr=본인 ,_target=대상
    protected List<GameCharacter> _Target_list = new List<GameCharacter>();

    public bool actionchecker()
    {
        return true;
    }

    public virtual void Active(int DMG,List<GameCharacter> l=null)
    {
        _Target_list = l;

            attack_by_type(DMG,_Target_list);

       
    }
    public virtual void Active(int ATK,int LV, ActionClass actionClass,List<GameCharacter> target)
    {
        _Target_list = target;


        switch (actionClass.ActionDesc)
        {
            case ActionDescription.Deal:

                foreach (GameCharacter chr in _Target_list)
                {
                    chr.execute_Hit_handler(actionClass.getfigure(LV, ATK));
                }
                break;
            case ActionDescription.Buff:
                foreach (GameCharacter chr in _Target_list)
                {
                    BuffTarget(chr, actionClass.bufftype, actionClass.getfigure(LV), actionClass.getEffectPower(LV));
                }
                break;
            case ActionDescription.Debuff:
                foreach (GameCharacter chr in _Target_list)
                {
                    DebuffTarget(chr, actionClass.debufftype, actionClass.getfigure(LV), actionClass.getEffectPower(LV));
                }
                break; 
            case ActionDescription.Heal:
                int healfigure= actionClass.getfigure(LV, ATK);
                foreach (GameCharacter chr in _Target_list)
                {
                    chr.execute_Heal_handler(actionClass.getfigure(LV, ATK));
                }
                break;
        }
        
        





    }
    void DebuffTarget(GameCharacter target, DebuffType t, int figure, float EffectPower)
    {

        switch (t)
        {
            case DebuffType.none:
                break;
            case DebuffType.forced:
                target.forced(target.transform.forward * -1 * figure, EffectPower);
                break;
           
            case DebuffType.ATKHit:
                target.GetStatus(new ATKHit(target, figure, EffectPower));
                break;
            case DebuffType.DamageSuppement:
                target.GetStatus(new DamageSuppement(target, figure, EffectPower));
                break;
            

        }
    }
    void BuffTarget(GameCharacter target, BuffType t,int figure,float EffectPower)
    {

        switch (t)
        {
            case BuffType.none:
                break;
            
            case BuffType.ATKBoost:
                target.GetStatus(new ATKBoost(target, figure, EffectPower));
                break;
            
            case BuffType.DamageCut:
                target.GetStatus(new DamageCut(target, figure, EffectPower));
                break;

        }
    }


    virtual protected void attack_by_type(int ATK, List<GameCharacter>  obj = null)
    {
        _Target_list.Clear();
    }

   
}

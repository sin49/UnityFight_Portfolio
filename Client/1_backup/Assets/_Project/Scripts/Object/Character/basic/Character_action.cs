using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Serialization;
[Serializable]
public class Character_action
{

    public string skillname;
    public string skilldescription;

    public ActionType ActionType;

    public List<ActionClass> ActionDesc = new List<ActionClass>();
    List<float> ActionPowers { get; set; }
    List<float> Actiondurations { get; set; }

    public List<float> getActionpowers()
    {
        UpdateActionDescs();
        return ActionPowers;
    }
    public List<float> getActiondurations()
    {
        UpdateActionDescs();
        return Actiondurations;
    }
    public void UpdateActionDescs()
    {
        if (ActionPowers == null && Actiondurations == null)
        {
            ActionPowers = new List<float>();
            Actiondurations = new List<float>();
        }
        else if (ActionPowers.Count!=0) {
            return;
        }
        
       


        foreach (ActionClass ac in ActionDesc)
        {
            if (ac.getfigurepercent(skillLV) != 0)
                ActionPowers.Add(ac.getfigurepercent(skillLV));
            if (ac.getEffectPower(skillLV) != 0)
                Actiondurations.Add(ac.getEffectPower(skillLV));
        }


       
    }



    public int skillLV;
    
    public int SkillUpgrradefee { get {return 1000 + 1000 * (2 * skillLV); } }



    public float init_time;//실행 전 대기 시간
    public string action_name;



    public Team target_team;//player==우리팀,enemy==상대방
    public string action;
   




    public void UpgradeSkill()
    {
        skillLV++;
      
        if (ActionPowers != null && Actiondurations != null)
        {
            ActionPowers.Clear();
            Actiondurations.Clear();
        }
    }

    public Character_action(Team t = Team.Enemy, float init_time = 0.2f,
    Priority p = Priority.Distance, PrioritySort m = PrioritySort.highest, int
        targetmmbernumber = 1)
    {
        ActionDesc = new List<ActionClass>();
        skillname = "test";

        skilldescription = $"test description 공격력의 % 만큼 블라블라";
        this.action = null;
        target_team = t;
        this.init_time = init_time;
       
        action = "attack_basic";

    }
    public Character_action(iAct action,Team t=Team.Enemy,float init_time=0.2f,
    Priority p=Priority.Distance, PrioritySort m = PrioritySort.highest,int
        targetmmbernumber=1)
    {
        this.action = action.GetType().Name;
        target_team = t;
        this.init_time = init_time;
     
    }
   
    iAct act { get; set; }
   
    public void Invoke(int DMG,GameCharacter c) {
        if (action == null)
        {
            return;
        }
        try
        {
            if (act == null)
            {
                act = Activator.CreateInstance(Type.GetType(action)) as iAct;
            }

            List<GameCharacter>[] Targets=new List<GameCharacter>[ActionDesc.Count];
            for(int i = 0; i < Targets.Length; i++)
            {
                Targets[i] = ActionDesc[i].Target(c);

            }
            string n;
            n = $"{action_name}이 {c.name} 측으로 실행 됨\n" +
                $"액션 대상:";
            foreach (List<GameCharacter> targetlist in Targets)
            {
                foreach (GameCharacter target in targetlist)
                {
                    n += target.name + " ";
                }
            }
            Debug.Log(n);

            for (int i = 0; i < ActionDesc.Count; i++)
            {
                act.Active(DMG, skillLV, ActionDesc[i], Targets[i]);
                Debug.Log("액션 실행 중" + i);

            }
        }
        catch
        {
            Debug.Log($"불려오기 실패: 시도 함수{action}");
            return;
        }
       
    }//실행
 
   
}
[Serializable]
public class ActionClass
{
    public ActionClass()
    {
        figure = 100;
        target_team = Team.Enemy;
        TargetMemberNumber = 1;
    }
    public ActionDescription ActionDesc;
    public float figure;
    public float figureByMaxLV;
    public float EffectPower = 0;
    public float EffectPowerByMaxLV = 0;
    public float getEffectPower(int Lv)
    {
        return (float)Math.Round((EffectPower + (EffectPowerByMaxLV / 9 * (Lv-1))),2);
    }
    public BuffType bufftype = BuffType.none;
    public DebuffType debufftype = DebuffType.none;
    public Priority priority;
    public PrioritySort PrioritySort;
    public AttackType attacktype=AttackType.single;
    public float attacksize=0;
    public int TargetMemberNumber;
    public Team target_team;//player==우리팀,enemy==상대방
    public float getfigurepercent(int LV)
    {
        return  (int)Mathf.Round(figure + Mathf.Round(figureByMaxLV / 9) * (LV- 1)) ;
    }
    public int getfigure(int n,int atk=1)
    {
        return (atk*Mathf.RoundToInt(figure + Mathf.Round(figureByMaxLV / 9) * (n - 1)))/100;
    }
    public List<GameCharacter> Target(GameCharacter c)
    {
     return   PrioritySystem.Character_Priority.instance.get_Characters(
                priority, PrioritySort, TargetMemberNumber, c, target_team,attacktype,attacksize);
    }
}
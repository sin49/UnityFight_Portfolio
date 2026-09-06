using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class CharacterInformation 
{
    public CharacterInformation()
    {
        normalattack = new Character_action();
        skills = new Character_action[0];
        LB = new Character_action();
    }

    public string id;
    public string name;
    public string Desc;
    public int BaseHP;
    public int BaseATK;
    public float HP_Grownth;
    public float ATK_Grownth;
    public int distancenumber;
    public int BaseGrade;


    public Position position;



    public Character_action normalattack;

    public Character_action[] skills;

    public void getSkillLV(PlayerData d)
    {
        if (d.skillLV == null || d.skillLV.Length != skills.Length)
        {
            d.skillLV = new int[skills.Length];
            for(int n = 0; n < d.skillLV.Length; n++)
            {
                d.skillLV[n] = 1;
            }
            if (d.SpecialLV == 0)
                d.SpecialLV = 1;
        }
        for(int i = 0; i < d.skillLV.Length; i++)
        {
            skills[i].skillLV = d.skillLV[i];
        }
        LB.skillLV = d.SpecialLV;
    }

    public Character_action LB;

    public List<string> actionset = new List<string>();
}

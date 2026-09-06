using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1_Skill1 : attack_basic
{
    //가장 가까운 대상에게 공격력의 125%+(레벨당 8%)의 피해를 줌

    protected override void attack_by_type(int ATK, List<GameCharacter> obj_list)
    {
        GameCharacter chr = obj_list[0];

        chr.execute_Hit_handler(ATK*2);
        chr.forced(chr.transform.forward*3, 1.25f);
        base.attack_by_type(ATK, obj_list);
    }

}

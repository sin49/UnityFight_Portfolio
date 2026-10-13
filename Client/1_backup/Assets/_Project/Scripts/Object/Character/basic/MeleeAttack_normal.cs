using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack_normal : attack_basic
{
    
   public MeleeAttack_normal()
    {
       
    }
    protected override void attack_by_type( int ATK, List<GameCharacter> obj_list)
    {
        if (obj_list!=null)
        {
            GameCharacter chr = obj_list[0];

            chr.execute_Hit_handler(ATK);

            base.attack_by_type(ATK, obj_list);
        }
        else
            return;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitBurst : attack_basic
{
    public override void Active(int DMG, List<GameCharacter> l = null)
    {
        on_LB_situaltion();
        base.Active(DMG, l);
        off_LB_situaltion();
    }
   
    //핵심! LB발동 시 LB의 대상과 선언 대상외에는 전원 비활성화
    //LB발동한 캐릭터 이외의 캐릭터의 에니메이션 corutine상태를 보관하고 idle&피격만 발동시키게 만들기
    public virtual void on_LB_situaltion()
    {

      
    }

    public virtual void off_LB_situaltion()
    {
        
    }
}

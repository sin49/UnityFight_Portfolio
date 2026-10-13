using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceItemIconUI : ItemIconUI
{


    public void setItem(string s)
    {
    


        item i = GameManager.instance.Account.get_item(s) == null ?
            new item() { itemid = s,Amount=0,data= LoadManager.instance.getitem("I05") } : GameManager.instance.Account.get_item(s);
        itemIcon.sprite = i.data.itemSprite;
        d = i;
    }
   
   
}

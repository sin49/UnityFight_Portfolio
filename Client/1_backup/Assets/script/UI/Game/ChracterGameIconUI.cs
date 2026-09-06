using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChracterGameIconUI : CharacterIconUI
{
    delegate void LBAct();
    LBAct a;
    protected override void buttoninput()
    {
        Debug.Log("delegate ½ÇÇà" + a.ToString());
        a();
    }
    protected override void Awake()
    {
        btn = this.gameObject.GetComponent<Button>();
        btn.gameObject.SetActive(false);
        base.Awake();
    }
 
    public void set_sprite_button(GameCharacter chr)
    {
       btn.onClick.RemoveAllListeners();
  
        btn.onClick.AddListener(() => { btn.interactable = false;
            buttoninput(); });
        a = chr.use_LB;
        
        portrait.sprite = chr.chr_sprite;
      btn.gameObject.SetActive(true);
        btn.interactable = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CharacterGameStatusUI : character_status_ui
{
    public ChracterGameIconUI iconUI;
    public GameObject sprite_mask;
    //public GameObject frame;
    Animator ani;
    private void Awake()
    {
       
        HP_slider.gameObject.SetActive(false);
        //frame.SetActive(false);
        ani =this.GetComponent<Animator>();
    }
    public override void update_information(int a, GameCharacter character)
    {
        if (a < 0)
        {
            sprite_mask.SetActive(true);
           
            return;
        }
       
        base.update_information(a, character);
       
        //if (LB_slider != null && LB_slider.check_vaule() == 0)
        //{
        //    if (character.is_LB_can_use_anywhere)
        //        iconUI.btn.interactable = true;
        //    else if (character.is_target_on_distance)
        //        iconUI.btn.interactable = true;
        //}
        //else
        //    iconUI.btn.interactable = false;



    }
    public void enable_icon_ui(GameCharacter chr)
    {
       iconUI.set_sprite_button(chr);
        active_icon_UI();
        ani.Play("chr_icon_On");
    }
   
    void active_icon_UI()
    {
        HP_slider.gameObject.SetActive(true);
        //frame.SetActive(true);
    }
   
}

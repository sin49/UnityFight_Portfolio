using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class character_status_ui : MonoBehaviour, Character_observer
{
    public status_slider HP_slider;
    public status_slider LB_slider;
    public virtual void update_information(int a = 0, GameCharacter character = null)
    {
        if (HP_slider != null)
        {
            HP_slider.max_vaule = character.MAX_hp;
            HP_slider.current_vaule = a;
        }
        if (LB_slider != null)
        {
            LB_slider.max_vaule = character.LB_gauge_max;
            LB_slider.current_vaule = character.LB_gauge;
        }
    }

   
}

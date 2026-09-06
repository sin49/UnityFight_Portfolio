using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class character_slider : character_status_ui
{
    public GameObject Character_transform;
    public override void update_information(int a=0, GameCharacter character=null)
    {
        base.update_information(a, character);
        if (Character_transform == null)
        {
            Character_transform = character.gameObject;
         
        }
        if (a < 0)
        {
            this.gameObject.SetActive(false);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Character_transform != null)
            this.transform.position = Camera.main.WorldToScreenPoint(Character_transform.transform.position + new Vector3(0, 1, 0.5f));
    }
}

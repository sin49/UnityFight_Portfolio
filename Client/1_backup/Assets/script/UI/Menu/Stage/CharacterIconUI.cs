using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterIconUI : MonoBehaviour
{
    public Image portrait;
    public Button btn;
    protected PlayerData data;
    protected virtual void buttoninput()
    {

    }
   protected virtual void Awake()
    {
        btn.onClick.AddListener(buttoninput);
    }

  
    protected virtual void Updatedata()
    {
  if(data != null)
            portrait.sprite = data.charactersprite;
    }
}

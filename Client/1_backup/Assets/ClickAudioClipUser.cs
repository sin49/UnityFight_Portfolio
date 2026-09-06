using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickAudioClipUser : AudioClipUser
{
   

    
    public override void PlayClickSound()
    {
        if(instance != null)
        {
            instance.PlayClickSound();
        }
        else
        {
            Debug.Log("Can't find UISoundPlayer");
        }
    }
}

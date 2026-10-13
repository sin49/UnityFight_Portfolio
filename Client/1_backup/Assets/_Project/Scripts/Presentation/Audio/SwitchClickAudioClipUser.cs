using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchClickAudioClipUser : AudioClipUser
{
   
         public override void PlayClickSound()
    {
        if (instance != null)
        {
            instance.PlaySwitchClickSound();
        }
        else
        {
            Debug.Log("Can't find UISoundPlayer");
        }
    
}
}

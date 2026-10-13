using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AudioClipUser : MonoBehaviour
{
   protected UIAudioPlayer instance;
    private void Start()
    {
        instance = UIAudioPlayer.instance;
        Button b;
        if (TryGetComponent<Button>(out b))
        {
            b.onClick.AddListener(PlayClickSound);
        }
    }
    public abstract void PlayClickSound();

}

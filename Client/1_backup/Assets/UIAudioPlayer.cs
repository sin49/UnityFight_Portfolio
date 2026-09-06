using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UIAudioPlayer : Audio_Player
{
    [Header("Ding-Dong Click")]
    public AudioClip ClickAudioClip;
   public float ClickAudioClipStartTime = 1.3f;
   public float ClickAudioClipLength = 0.4f;
    public float ClickVolume;
    [Header("Switch Click")]
    public AudioClip ClickAudioClip2;
    public float Click2AudioClipStartTime ;
    public float Click2AudioClipLength ;
    public float Click2Volume;


    public static UIAudioPlayer instance;
    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
            instance = this;

    }

    public void PlayClickSound()
    {
        Stop();
        m_AudioSource.volume = ClickVolume;
        Play(ClickAudioClip, ClickAudioClipStartTime, ClickAudioClipLength);
    }
    public void PlaySwitchClickSound()
    {
        Stop();
        m_AudioSource.volume = Click2Volume;
        Play(ClickAudioClip2, Click2AudioClipStartTime, Click2AudioClipLength);
    }

}

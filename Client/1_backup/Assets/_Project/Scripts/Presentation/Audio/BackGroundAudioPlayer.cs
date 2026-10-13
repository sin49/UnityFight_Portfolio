using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class BackGroundAudioPlayer : Audio_Player
{

    [Header("타이틀")]
    public float TitleLoop_Time;
    public float TitleLoopLength;
    [Range(0,1)]
    public float TitleBackGroundVolume;
    public AudioClip TitleBGAudioClip;

    [Header("메인")]
    public float mainLoop_Time;
    public float mainLoopLength;
    [Range(0, 1)]
    public float mainBackGroundVolume;
    public AudioClip mainBGAudioClip;

    [Header("스테이지")]
    public float stageLoop_Time;
    public float stageLoopLength;
    [Range(0, 1)]
    public float stageBackGroundVolume;
    public AudioClip stageBGAudioClip;


    public static BackGroundAudioPlayer instance;
    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
            instance = this;
        
    }
  
    protected void Play(AudioClip clip,float Loop_Time,float BackGroundVolume,float LoopLength)
    {
        m_AudioSource.loop = true;

        m_AudioSource.volume = BackGroundVolume;
        base.Play(clip,Loop_Time,LoopLength,true);

    }
 
   
    public void PlayTitleBG()
    {
        Play(TitleBGAudioClip,TitleLoop_Time,TitleBackGroundVolume,TitleLoopLength);
    }
    public void PlaymainBG()
    {
        Play(mainBGAudioClip, mainLoop_Time, mainBackGroundVolume, mainLoopLength);
    }
    public void PlaystageBG()
    {
        Play(stageBGAudioClip, stageLoop_Time, stageBackGroundVolume, stageLoopLength);
    }
    public void PauseBG()
    {
        Debug.Log("BG is Pause");
        m_AudioSource.Pause();
    }

}

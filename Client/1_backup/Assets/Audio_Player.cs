using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
//using static Unity.Barracuda.TextureAsTensorData;

public class Audio_Player : MonoBehaviour
{
    
  
    protected AudioSource m_AudioSource;
    protected float current_Audio_Length;
    public AudioMixerGroup m_mixer;
   protected AudioClip CurrentAudioClip;
   
    public virtual void Stop()
    {
        m_AudioSource.Stop();
        StopAllCoroutines();
    }
    protected virtual void Play(AudioClip clip,float Start_time=0,float length=0,bool loop=false)
    {
        m_AudioSource.Pause();
      
        CurrentAudioClip = clip;
        m_AudioSource.clip = CurrentAudioClip;
        if (length == 0)
            length = clip.length;
        m_AudioSource.time =
           Start_time;
   
            m_AudioSource.Play();
        StartCoroutine(Audio_Pause_by_Length(Start_time, length, loop));
       
    }
    IEnumerator Audio_Pause_by_Length(float start_time,float length,bool loop)
    {
        yield return new WaitForSeconds(length);
        if (loop)
        {
            float speed = m_AudioSource.volume / 2 * Time.deltaTime;
            while (m_AudioSource.volume > 0.0005f)
            {
                yield return null;
                m_AudioSource.volume -= speed;
            }
            m_AudioSource.Stop();

            Play(CurrentAudioClip,start_time,length,loop);
        }else
            m_AudioSource.Stop();
    }
    protected virtual void Awake()
    {
        if (!TryGetComponent<AudioSource>(out m_AudioSource))
        {
            m_AudioSource = this.gameObject.AddComponent<AudioSource>();
            m_AudioSource.maxDistance = m_AudioSource.minDistance += .1f;
            m_AudioSource.outputAudioMixerGroup = m_mixer;
        }
       
    }
   
}

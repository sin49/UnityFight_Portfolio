using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class Character_Audio_Player : Audio_Player
{
  public AudioClip HitClip;
  
    public void PlayHitClip(Character_action action)
    {
        StartCoroutine(PlayHit(action));
    }
    
     IEnumerator PlayHit(Character_action action)
    {
       
     
        yield return new WaitForSeconds(action.init_time);
        m_AudioSource.volume = 0.75f;
        Play(HitClip);
    }

}

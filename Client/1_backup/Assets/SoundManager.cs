using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> AudioClips;
    Dictionary<string, AudioClip> AudioDict = new Dictionary<string, AudioClip>();
    public AudioMixerGroup m_mixer;


    private void Awake()
    {
        AudioDict.Clear();
        AudioDict = AudioClips.ToDictionary(a => a.name, a => a);
    }

}

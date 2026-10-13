using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDisable : MonoBehaviour
{
    ParticleSystem ParticleSystem;
   public float duration;
    private void Awake()
    {
        ParticleSystem = GetComponent<ParticleSystem>();

    }
    private void OnEnable()
    {
        StartCoroutine(AutoDisable());
    }
    IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(duration);
        this.gameObject.SetActive(false);
    }

}

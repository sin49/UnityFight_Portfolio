using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Particle_System : MonoBehaviour
{
    public ParticleSystem HitEffect;
    public ParticleSystem DieEffect;
   
    public void CreateHitEffect(Vector3 Position)
    {
        Instantiate(HitEffect, Position, HitEffect.transform.rotation);
    }
    public void CreateDieEffect()
    {
        Instantiate(DieEffect,this.transform.position,DieEffect.transform.rotation);
    }
}

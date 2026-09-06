using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCharacterStanding : MonoBehaviour
{
    GameObject starcharacter;
    float CharacterRotateSpeed;
    public Transform ChracterStanding;
    Animator ani;
    private void OnEnable()
    {
        if (starcharacter != null)
        {
            ani = starcharacter.transform.GetChild(0).GetComponent<Animator>();
            ani.SetBool("NotBattle", true);
        }
    }
    public void createstarcharacter(GameObject obj,float speed)
    {
        starcharacter = Instantiate(obj, ChracterStanding);
        ani = starcharacter.transform.GetChild(0).GetComponent<Animator>();
        ani.SetBool("NotBattle", true);
        CharacterRotateSpeed = speed;
        if (starcharacter.GetComponent<Rigidbody>() != null)
            Destroy(starcharacter.GetComponent<Rigidbody>());
        if (starcharacter.GetComponent<GameCharacter>() != null)
            Destroy(starcharacter.GetComponent<GameCharacter>());

      
        starcharacter.transform.position = ChracterStanding.transform.position;
    }
    private void FixedUpdate()
    {
        if(starcharacter!=null)
            starcharacter.transform.Rotate(new Vector3(0, CharacterRotateSpeed, 0));
    }
}

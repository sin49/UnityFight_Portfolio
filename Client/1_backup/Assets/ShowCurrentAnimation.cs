using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShowCurrentAnimation : MonoBehaviour
{
    Animator ani;
    float timer;
    Vector3 PosBuffer;
    Quaternion rotationBuffer;
    Action Handler=null;
    void Start()
    {
        ani = this.GetComponent<Animator>(); 
    }
    void getBuffer()
    {
        PosBuffer = transform.position;
        rotationBuffer = transform.rotation;
    }
   void returnBuffer()
    {
        transform.position = PosBuffer;
        transform.rotation = rotationBuffer;
        Debug.Log("Buffer return");
  
    }
    // Update is called once per frame
    void Update()
    {
        if (Handler != null)
            Handler.Invoke();
        if(ani.GetCurrentAnimatorClipInfo(0).Length>0)
        Debug.Log(this.gameObject.name + " play animation :" +
            ani.GetCurrentAnimatorClipInfo(0)[0].clip.name);
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 120), "Move"))
        {
            ani.SetBool("OnMove", true);
        }
        if (GUI.Button(new Rect(0, 150, 200, 120), "NotMove"))
        {
            ani.SetBool("OnMove", false);
        }
        if (GUI.Button(new Rect(0, 300, 200, 120), "Stun1sec"))
        {
            ani.SetFloat("stun", 1);
        }
        if (GUI.Button(new Rect(0, 450, 200, 120), "nostun"))
        {
            ani.SetFloat("stun", 0);
        }
        if (GUI.Button(new Rect(0, 600, 200, 120), "Hitted"))
        {
            ani.Play("Damaged");
        }
        if (GUI.Button(new Rect(0, 750, 200, 120), "Die"))
        {
            
            ani.SetBool("Die2", true);
            ani.SetTrigger("Die");
        }
        if (GUI.Button(new Rect(0, 900, 200, 120), "NoDie"))
        {
            ani.SetBool("Die2", false);
        }
        if (GUI.Button(new Rect(200, 0, 200, 120), "salute"))
        {
            ani.Play("Salute");
        }
        if (GUI.Button(new Rect(200, 150, 200, 120), "normal"))
        {
            ani.Play("normal");
        }
        if (GUI.Button(new Rect(200, 300, 200, 120), "skill1"))
        {
            getBuffer();
            ani.Play("skill1");
            timer = ani.GetCurrentAnimatorClipInfo(0)[0].clip.length;

            Handler = (new Action(() =>
            {
                timer -= Time.deltaTime;
          
                if (
               timer<0 && ani.GetCurrentAnimatorClipInfo(0)[0].clip.name=="Idle")
                {
                    Handler = null;

                    returnBuffer();
                }
            }));
        }
        if (GUI.Button(new Rect(200, 450, 200, 120), "special"))
        {
            getBuffer();
            ani.Play("Special");
            timer = ani.GetCurrentAnimatorClipInfo(0)[0].clip.length;

            Handler = (new Action(() =>
            {
                timer -= Time.deltaTime;

                if (
               timer < 0 && ani.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle")
                {
                    Handler = null;

                    returnBuffer();
                }
            }));
        }
    }
}

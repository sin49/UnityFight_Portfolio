using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBackGroundMove : MonoBehaviour
{
    //¹æÇâ?
    public float Speed;


    void Update()
    {
        

        this.transform.Translate(new Vector3(0,Speed* Time.deltaTime, 0));

        if(this.transform.position.y>107.92f)
        {
            
            this.transform.position = this.transform.parent.position;
        }
    }
}

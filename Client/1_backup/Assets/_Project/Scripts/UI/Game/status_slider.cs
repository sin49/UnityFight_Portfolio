using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class status_slider : MonoBehaviour
{
    //[HideInInspector]
    public float current_vaule;
    //[HideInInspector]
    public float max_vaule = 0;

    protected Slider slider;

    private void Awake()
    {

        slider = this.GetComponent<Slider>();

    }
    protected virtual void Update()
    {
        slider.value = (float)current_vaule / (float)max_vaule;
    }
    //public float check_vaule()
    //{

    //    return  slider.value;
       
    //}
}

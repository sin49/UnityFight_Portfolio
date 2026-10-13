using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DoubleButtonSliderHandler : DoubleButtonHandlerUI
{
    public delegate void Input(float f);
    Input valueinput;
    public Slider slider;
    protected override void Update()
    {
        base.Update();
        slider.value = count;
    }
    public void SetSliderVauleChangeInput(Input action)
    {
        valueinput = action;
        slider.onValueChanged.AddListener((f) => { valueinput(f); });
        PlusButton.onClick.AddListener(() =>
        {
            slider.value = count;
            valueinput(count);
        });
        MinusButton.onClick.AddListener(() =>
        {
            slider.value = count;
            valueinput(count);
        });
    }

    //protected override void PlusButtonInput()
    //{
    //    base.PlusButtonInput();
      


      
    //}
    //protected override void MinusButtonInput()
    //{
    //    base.MinusButtonInput();
      
    //    slider.value = count;
      
    //}
    protected override void Awake()
    {
        base.Awake();
        slider.maxValue = MaxCount;
        slider.onValueChanged.AddListener((f) => { count = (int)f; });
    }
    protected override void OnEnable()
    {
        MinusButton.interactable = true;
        PlusButton.interactable = true;
        if (count == 0)
        {
            MinusButton.interactable = false;
        }else if (count == MaxCount)
        {
            PlusButton.interactable = false;
        }
    }
}

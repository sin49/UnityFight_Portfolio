using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class status_slider_reverse : status_slider
{
    protected override void Update()
    {
        slider.value = 1 - current_vaule / max_vaule;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TitleText : MonoBehaviour
{

    TextMeshProUGUI tmp;
    float x;
    bool operation;
    float TimeDelay=0.2f;
    float _Time;
    private void Awake()
    {
        tmp=this.GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        _Time += Time.deltaTime;
        if (_Time > TimeDelay)
        {
            if (!operation)
                x += 0.03f;
            else
                x -= 0.03f;
            if (x > 0.15f)
                operation = true;
            else if (x < 0.03)
                operation = false;
            tmp.outlineWidth = x;
            _Time = 0;
        }
    }
}

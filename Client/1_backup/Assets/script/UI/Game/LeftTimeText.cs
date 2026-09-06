using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeftTimeText : MonoBehaviour
{

    TextMeshProUGUI s;
    // Update is called once per frame
    private void Awake()
    {
        s = this.gameObject.GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        s.text = ((int)Stage._stage.left_time/60).ToString()+":" + ((int)Stage._stage.left_time % 60/10).ToString()+ ((int)Stage._stage.left_time % 60 % 10).ToString();
    }
}

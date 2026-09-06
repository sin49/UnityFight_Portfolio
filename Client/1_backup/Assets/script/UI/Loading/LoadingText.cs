using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LoadingText : MonoBehaviour
{
    TextMeshProUGUI loadingtext;
    float timer;
    private void Awake()
    {
        loadingtext = this.GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.5f)
        {
            timer = 0;
            if (loadingtext.text.Length < 10)
            {
                loadingtext.text += ".";
            }
            else
                loadingtext.text = "Loading";
        }
    }
}

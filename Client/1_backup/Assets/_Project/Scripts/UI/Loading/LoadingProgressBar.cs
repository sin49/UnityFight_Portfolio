using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LoadingProgressBar : MonoBehaviour
{
    Slider slider;
    public TextMeshProUGUI percentText;
    public TextMeshProUGUI LoadingText;
    string PeriodString;
    const float time = 0.25f;
    float _time = 0;
    private void Awake()
    {
        slider = this.GetComponent<Slider>();
        BackGroundAudioPlayer.instance.PauseBG();
    }
    private void OnEnable()
    {
        PeriodString = "";
        _time = 0;
    }
    private void Update()
    {
        slider.value = LoadManager.instance.Loadingprogress;
        slider.maxValue = LoadManager.instance.MaxLoadingprogress;
        int percent = Mathf.RoundToInt((slider.value / slider.maxValue) * 100);
        if (percent < 100)
            percentText.text = percent + "%";
        else if(percent>=100||percent<0)
            percentText.text = "0%";
        _time+= Time.deltaTime;
        if (_time < time)
        {
            PeriodString += ".";
            if (PeriodString.Length > 3)
                PeriodString = "";
            _time = 0;
            LoadingText.text = "Loading" + PeriodString;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using DataExecute;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SettingData{
   public int BGMVolume;
    public int SEVolume;
    public int GraphicSettingindex;
}
public class SettingUI : MonoBehaviour
{
    public Button BackButton;
    public DoubleButtonSliderHandler BGAudioSlider;
    public DoubleButtonSliderHandler SEAudioSlider;

    public Toggle HighGraphicOptionToggle;
    public Toggle MiddleGraphicOptionToggle;
    public Toggle LowGraphicOptionToggle;

    public TextMeshProUGUI AccountUIDText;

    public Button AccountUIDCopyButton;
    public Button AccountDeleteButton;
    public Button GoTitleButton;
    public GameObject invisibleBG;
    public DataInitUI datainitui;

    public void SetVolume()
    {
        BGAudioSlider.MaxCount = 100;
        SEAudioSlider.MaxCount = 100;
        BGAudioSlider.SetSliderVauleChangeInput((f) => {
            Debug.Log("BG 값 변경 감지됨 값:"+f);
            if (f != 0)
            {
                GameManager.instance.audiomixer.SetFloat("BG", Mathf.Log10(f) * 10);
            }
            else
            {
                GameManager.instance.audiomixer.SetFloat("BG", -80);
            }
            GameManager.instance._settingdata.BGMVolume = (int)f;
                GameManager.instance.ReWriteSetting("Sound", "BackGround", ((int)f).ToString());
            
           
        });

        SEAudioSlider.SetSliderVauleChangeInput((f) => {
            if (f != 0)
            {
                GameManager.instance.audiomixer.SetFloat("SE", Mathf.Log10(f) * 10);
            }
            else
            {
                GameManager.instance.audiomixer.SetFloat("SE", -80);
            }
                GameManager.instance._settingdata.BGMVolume = (int)f;
            GameManager.instance.ReWriteSetting("Sound", "SoundEffect", ((int)f).ToString());
        });
    }

   

    private void Awake()
    {

        BGAudioSlider.count = GameManager.instance._settingdata.BGMVolume;
        SEAudioSlider.count = GameManager.instance._settingdata.SEVolume;
        switch (GameManager.instance.GraphicSettingindex)
        {
            case 0:
                LowGraphicOptionToggle.isOn = true;
                break;
            case 1:
                MiddleGraphicOptionToggle.isOn = true;
                break;
            case 2:
                HighGraphicOptionToggle.isOn = true;
                break;
        }
        AccountUIDText.text = "UID:" + GameManager.instance.Account.uid;
        BackButton.onClick.AddListener(() =>
        {
            BackButtonInput();
        });
        AccountUIDCopyButton.onClick.AddListener(() => {
            GUIUtility.systemCopyBuffer=GameManager.instance.Account.uid;
            
        });
        GoTitleButton.onClick.AddListener(() =>
        {
            GameManager.instance.DisConnectServer();
            SceneManager.LoadScene("Title");
        });
        AccountDeleteButton.onClick.AddListener(() =>
        {
            datainitui.gameObject.SetActive(true);
            datainitui.setting = this;
        });
        LowGraphicOptionToggle.onValueChanged.AddListener((a) => { if (a) { GameManager.instance.GraphicSettingindex = 0;
                QualitySettings.SetQualityLevel(0, false);
            }
        });
        MiddleGraphicOptionToggle.onValueChanged.AddListener((a) => {
            if (a)
            {
                GameManager.instance.GraphicSettingindex = 1;
                QualitySettings.SetQualityLevel(1, false);
            }
        });
        HighGraphicOptionToggle.onValueChanged.AddListener((a) => {
            if (a)
            {
                GameManager.instance.GraphicSettingindex = 2;
                QualitySettings.SetQualityLevel(2, false);
            }
        });
        SetVolume();
    }

    void BackButtonInput()
    {
        this.gameObject.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LVUpUI : MonoBehaviour
{
    public TextMeshProUGUI LVText;
    public TextMeshProUGUI LVIntroduceText;
    public Button OKButton;
    private void Awake()
    {
        OKButton.onClick.AddListener(() =>
        {

            this.gameObject.SetActive(false);
            GameManager.instance.DeactiveGrayBG();
        });
    }

  public void ActiveLVUpUI(string LV,string PastStamina,
      string currentstamina)
    {
        LVText.text = LV;
        LVIntroduceText.text = "";
        if (PastStamina != currentstamina)
            LVIntroduceText.text += $"최대 스테미나 {PastStamina} 에서 {currentstamina} 로 증가";
        LVIntroduceText.text += $"\n스테미나 회복";
        this.gameObject.SetActive(true);
    }
}

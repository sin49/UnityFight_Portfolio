using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using menu;
using UnityEngine.UI;
using TMPro;
public class GameClearCharacterIcon : CharacterIconUI
{
    public Slider ExpSlider;
    public TextMeshProUGUI ExpText;
    public TextMeshProUGUI ExpGetText;
    public float AnimationTIme=1;
    public TextMeshProUGUI LVText;
    float GetEXP;
    float PastExp;
    int PastLV;
    public bool AnimCheck;
   
    public void skipAnim()
    {
        if (data == null) { AnimCheck = true; return; }
        StopAllCoroutines();
        AnimCheck = true;
        ExpSlider.maxValue = data.getmaxexp(data.LV);
        ExpSlider.value = data.EXP;
        ExpText.text = data.EXP + "/" + data.getmaxexp(data.LV);
        LVText.text = data.LV.ToString();
    }
    public void ActiveUI(int exp, PlayerData data)
    {
        this.data = data;
        this.PastLV = data.LV;
        this.PastExp = data.EXP;
        GetEXP = exp;
        ExpGetText.gameObject.SetActive(true);
        LVText.text = PastLV.ToString();
        ExpText.text = (int)PastExp + "/" + data.getmaxexp(PastLV);
        ExpGetText.text = "+"+GetEXP.ToString();
        ExpSlider.maxValue = data.getmaxexp(PastLV);
        ExpSlider.value = PastExp;
        portrait.sprite = data.charactersprite;
    }
  
   
   public IEnumerator ExpUpdate()
    {
        Debug.Log("Expupdate...");
        if (data == null)
        {
            Debug.Log("no data");
            AnimCheck = true;
            yield break;
        }
        float Speed = GetEXP / AnimationTIme;
        if (Speed < 1)
            Speed = 1;
        while (true)
        {
           

            PastExp+= Time.deltaTime * Speed;
           
            ExpSlider.value = PastExp;
       
            if (ExpSlider.value >= ExpSlider.maxValue)
            {
                ExpSlider.value = 0;
                ExpSlider.maxValue = data.getmaxexp(++PastLV);
                LVText.text = PastLV.ToString();
            }
            ExpText.text = (int)ExpSlider.value + "/" + ExpSlider.maxValue;
            if (GetEXP > 0)
            {
                GetEXP -= Time.deltaTime * Speed;
                ExpGetText.text = "+" + Mathf.RoundToInt(GetEXP).ToString();
            }
            else
            {
                ExpGetText.gameObject.SetActive(false);
            }
            if (ExpSlider.value >= data.EXP && PastLV == data.LV)
            {
                ExpSlider.value = data.EXP;
                AnimCheck = true;
                yield break;
            }
            yield return null;
        }
    }
   
}

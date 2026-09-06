using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class spawncharactericonUI : MonoBehaviour
{
    public Image portrait;
    public Button btn;
    public CharacterData data;
    public TextMeshProUGUI LVText;
    protected virtual void buttoninput()
    {
        GameManager.instance.ActiveChrPopUp(data.Name,  this.transform);
    }
    protected virtual void Awake()
    {
        btn.onClick.AddListener(buttoninput);
    }
    public void SetData(CharacterData data)
    {
        this.data = data;
        data.charactersprite = LoadManager.instance.getcharactersprite(data.id);

        updateUI();
    }
    void updateUI()
    {
        LVText.text = "LV"+data.LV;
        portrait.sprite = data.charactersprite;
    }

}

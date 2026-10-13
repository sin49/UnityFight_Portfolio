using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ErrorMessage : MonoBehaviour
{
    public TextMeshProUGUI Errormessage;
    public Button ErrorButton;
    public void Awake()
    {
        ErrorButton.onClick.AddListener(ErrorButtoninput);
    }
    public void activemessage(string ErrorMessage)
    {
        Errormessage.text = ErrorMessage;
        this.gameObject.SetActive(true);
    }
    public void ErrorButtoninput()
    {
       

        GameManager.instance.OnError = false;
        this.gameObject.SetActive(false);
        GameManager.instance.grayBG.SetActive(false);
        LoadManager.instance.LoadTitle();
    }
}

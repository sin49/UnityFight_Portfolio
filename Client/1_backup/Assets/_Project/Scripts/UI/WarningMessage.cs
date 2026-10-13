using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningMessage : MonoBehaviour
{
    public TextMeshProUGUI Warningmessage;
    public Button WarningButton;
    public Action ButtonAction;
    public void Awake()
    {
        WarningButton.onClick.AddListener(WarningButtoninput);
    }
    public void activemessage(string WarningMessage,Action action)
    {
        Warningmessage.text = WarningMessage;
        ButtonAction = action;
        this.gameObject.SetActive(true);
    }
    public void WarningButtoninput()
    {

        if (ButtonAction != null)
        {
            ButtonAction.Invoke();
            ButtonAction = null;
        }
       
        this.gameObject.SetActive(false);
   GameManager.instance.DeactiveGrayBG();
    }
}

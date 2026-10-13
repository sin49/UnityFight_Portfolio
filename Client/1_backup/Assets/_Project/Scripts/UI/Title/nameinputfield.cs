using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.UI;
public class nameinputfield : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI inputwarningtext;
    public Button inputButton;
    //public Button CancelButton;
    void Awake()
    {
        inputField.onValueChanged.AddListener(InputfieldvalueChnge);
        inputButton.interactable = false;
        inputButton.onClick.AddListener(buttonListner);
        inputButton.onClick.AddListener(setName);
        //CancelButton.onClick.AddListener(CancelInput);
        //CancelButton.onClick.AddListener(buttonListner);
       
    }
   void CancelInput()
    {
   if(!LoadManager.instance.onload)
            inputField.text = null;
    }
   void buttonListner()
    {
        if(!LoadManager.instance.onload)
            this.gameObject.SetActive(false);

    }
    int sliceSpcialChacharacter(char c)
    {
        if ((c>47&&c < 58) ||( c > 64 && c < 91 )|| c > 96)
            return c;
        else
            return 0;
    }
    void InputfieldvalueChnge(string s)
    {
        if (s.Length == 0)
            return;
        if (sliceSpcialChacharacter(s[s.Length-1]) == 0)
        {
            inputwarningtext.text = "사용할 수 없는 문자입니다!";
        }
        else if (s.Length > 8)
        {
            inputwarningtext.text = "8글자를 초과할 수 없습니다!";
        }
        else
        {
            inputwarningtext.gameObject.SetActive(false);
            return;
        }

       inputField.text= s.Remove(s.Length-1);
            inputwarningtext.gameObject.SetActive(true);
    }
    public string getName()
    {
        return inputField.text;
    }
    private void Update()
    {
        if (inputField.text.Length > 0)
            inputButton.interactable = true;
        else
            inputButton.interactable = false;
    }
    private void setName()
    {
        if(GameManager.instance.Account!=null)
        GameManager.instance.Account.username = inputField.text;
    }


   
}

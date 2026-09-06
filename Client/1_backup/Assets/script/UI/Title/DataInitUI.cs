using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataInitUI : MonoBehaviour
{
    public Button OKButton;
    public Button CancelButton;
    [HideInInspector]
   public TitleScreen title;
    [HideInInspector]
    public  SettingUI setting;
   
    public Button CheckButton;
    public TextMeshProUGUI CheckText;

    private void Awake()
    {
        OKButton.onClick.AddListener(OKButtonInput);
        CancelButton.onClick.AddListener(CancelButtonInput);
        CheckButton.onClick.AddListener(CheckButtonInput);
    }
    public void OKButtonInput()
    {
        FIleIOManager.DeleteConnectionCache();
        if (title != null)
            title.UpdateTitle();
        CheckText.text = "초기화 되었습니다.";
        OKButton.transform.parent.gameObject.SetActive(false);
        CheckButton.transform.parent.gameObject.SetActive(true);
      
        //SceneManager.LoadScene("Title");
    }
    public void CheckButtonInput()
    {
        OKButton.transform.parent.gameObject.SetActive(true);
        CheckButton.transform.parent.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
       if(title!=null)
        title.InvisibleBG.SetActive(false);
        else if(setting!=null)
        {
            setting.invisibleBG.SetActive(false);
            if (FIleIOManager.cache == null)
            {
                GameManager.instance.DisConnectServer();
                SceneManager.LoadScene("Title");
            }
        }
    }
    public void CancelButtonInput()
    {
        CheckText.text = "취소 되었습니다.";
        OKButton.transform.parent.gameObject.SetActive(false);
        CheckButton.transform.parent.gameObject.SetActive(true);
    }
}

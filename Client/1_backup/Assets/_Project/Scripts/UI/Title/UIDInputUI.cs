using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class UIDInputUI : MonoBehaviour
{
    //UI1
    public TMP_InputField UIDInputfield;
    public Button CancelButton;
    public Button OkButton;
    //Loading
    public GameObject LoadingUI;
    //UI2
    public TextMeshProUGUI accounttext;
    public Button accountOKButton;
    public Button accountcancleButton;
    string accountuid;
    //uid3
    public TextMeshProUGUI checktext;
    public Button CheckButton;

    public TitleScreen title;
    private void OnDisable()
    {
        UIDInputfield.text = null;
    }
    private void Awake()
    {
        CancelButton.onClick.AddListener(CancelButtonInput);
        OkButton.onClick.AddListener(OKButtonInput);
        accountOKButton.onClick.AddListener(OKButtonInput2);
        accountcancleButton.onClick.AddListener(CancelButtonInput2);
        CheckButton.onClick.AddListener(CheckButtonInput);
    }
    private void Update()
    {
        if (UIDInputfield.text.Length != 12)
            OkButton.interactable = false;
        else
            OkButton.interactable = true;
    }

    public void CheckButtonInput()
    {
        OkButton.transform.parent.gameObject.SetActive(true);
        UIDInputfield.text = null;
        accountOKButton.transform.parent.gameObject.SetActive(false);
        CheckButton.transform.parent.gameObject.SetActive(false);
        title.UpdateTitle();
        this.gameObject.SetActive(false);
        title.InvisibleBG.SetActive(false);
    }
    public void OKButtonInput()
    {
        StartCoroutine(TryFindAccount(UIDInputfield.text));
    }
    public void OKButtonInput2()
    {
        UserAccountData data = new UserAccountData() { uid=accountuid};
        FIleIOManager.MakeConnectionCache(data);
        checktext.text = "계정을 성공적으로 불려왔습니다.";
        accountOKButton.transform.parent.gameObject.SetActive(false);
        checktext.transform.parent.gameObject.SetActive(true);
    }
    IEnumerator TryFindAccount(string uid)
    {
        OkButton.transform.parent.gameObject.SetActive(false);
        LoadingUI.SetActive(true);
       yield return StartCoroutine(GameManager.instance.TryConnectServer());
        if (!GameManager.instance.singlemode && !GameManager.instance.OnServerConnect)
        {
            LoadingUI.SetActive(false);
            checktext.text = "서버와의 통신이 원활하지 않습니다.";
            checktext.transform.parent.gameObject.SetActive(true);
            yield break;
        }
        yield return StartCoroutine(GameManager.instance.ConnectAccountCorutine(uid));
        //yield return StartCoroutine(GameManager.instance.ConnectAccountCorutine(uid));
        LoadingUI.SetActive(false);
        bool checker = !GameManager.instance.OnError && GameManager.instance.Account != null;

        if (checker)
        {

      
            accounttext.text = $"이름:{GameManager.instance.Account.username}\n" +
                $"LV:{GameManager.instance.Account.accountlv}";
            accountuid = GameManager.instance.Account.uid;
            GameManager.instance.Account = null;

            accounttext.transform.parent.parent.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("계정 없");
            checktext.text = "UID와 일치한 계정을 찿을 수 없습니다.";
            checktext.transform.parent.gameObject.SetActive(true);
        }
        GameManager.instance.DisConnectServer();
    }
    public void CancelButtonInput()
    {
        this.gameObject.SetActive(false);
        title.InvisibleBG.SetActive(false);
    }
    public void CancelButtonInput2()
    {
        checktext.text = "취소 되었습니다.";
        FIleIOManager.DeleteConnectionCache();
        accountOKButton.transform.parent.gameObject.SetActive(false);
        checktext.transform.parent.gameObject.SetActive(true);
     
    }
}


using System.Collections;

using TMPro;

using UnityEngine;

using UnityEngine.UI;
using Random = UnityEngine.Random;


public class TitleScreen : MonoBehaviour
{
    public GameObject TitleParticleSystem;

    public nameinputfield inputField;
    public TextMeshProUGUI UID;
    public TextMeshProUGUI Ver;
    public TextMeshProUGUI accountChecker;
    public GameObject InvisibleBG;
    public Button StartButton;
    public Button TitleButton_UID;
    public TextMeshProUGUI TitleButton_UID_Text;
    public DataInitUI InitUI;
    public UIDInputUI UIDInputUI;


    ConnectionCache cache;



    public void UpdateTitle()//초기화
    {
        inputField.gameObject.SetActive(false);

        accountChecker.gameObject.SetActive(false);
        //TitleParticleSystem.SetActive(true);
        StartButton.gameObject.SetActive(true);

        cache = FIleIOManager.cache;
        if (cache == null)
        {
            UID.text = null;
            UID.gameObject.SetActive(false);

            TitleButton_UID_Text.text = "계정\n연결";
            
        }
        else
        {
            UID.gameObject.SetActive(true);
            UID.text = "UID: " + cache.UID;
            TitleButton_UID_Text.text = "초기화";

        }
    }
 
    public void UIDTitleButtonInput()//접속 버튼 눌림
    {
        InvisibleBG.SetActive(true);
        if (cache == null)
        {
            UIDInputUI.gameObject.SetActive(true);
            UIDInputUI.title = this;
        }else
        {
            InitUI.gameObject.SetActive(true);
            InitUI.title = this;
        }
    }
   
    private void Awake()
    {
       
       
        Ver.text = "Ver: " + Application.version;
        TitleButton_UID.onClick.AddListener(UIDTitleButtonInput);
        BackGroundAudioPlayer.instance.PlayTitleBG();
      
        UpdateTitle();
    }


    IEnumerator ConnectAccount(string UID)//계정 연결중 코루틴
    {

        accountChecker.gameObject.SetActive(true);
        accountChecker.text = "연결 중";
        yield return StartCoroutine(GameManager.instance.FakeConnectAccountCorutine(cache.UID));//gmaemanageer에 계정을 연결하는 코루틴을 실행 후 대기
        //yield return StartCoroutine(GameManager.instance.ConnectAccountCorutine(cache.UID));//gmaemanageer에 계정을 연결하는 코루틴을 실행 후 대기
 
            if (GameManager.instance.Account != null)//연결됨
            {
                StartCoroutine(LoadGame());
            }
            else//연결안됨
            {

                accountChecker.gameObject.SetActive(false);
                GameManager.instance.ActiveError("서버와의 연결이 원활하지 않습니다.");
            }
        

    }
    IEnumerator CreateAccount()//uid가 없으면 계정 생성
    {

        inputField.gameObject.SetActive(true);
        yield return new WaitUntil(() => !inputField.gameObject.activeSelf);
        if(inputField.inputField.text != null)
        {
            Debug.Log("체크1");
            string username = inputField.getName();
            Debug.Log("입력된 이름" + username);
            accountChecker.gameObject.SetActive(true);
            accountChecker.text = "계정 생성 중";
            if(!GameManager.instance.singlemode)
            yield return StartCoroutine(GameManager.instance.createaccountcorutine(username));
            else
                yield return StartCoroutine(GameManager.instance.createaccountcorutineSingle(username));
            //uid 입력 후 계정 생성
            if (GameManager.instance.Account.uid!=null&&
                GameManager.instance.Account.uid.Length!=0)
            {
               
                accountChecker.gameObject.SetActive(false);

              
                    Debug.Log(GameManager.instance.Account.uid + "계졍 생성됨");

                    StartCoroutine(LoadGame());
           
            }
            else
            {

                accountChecker.gameObject.SetActive(false);
                GameManager.instance.ActiveError("서버와의 연결이 원활하지 않습니다.");
            }
            yield break;
        }
        else
        {
      
            yield break;
        }
        

    }



    public void StartButtonInput()//시작버튼 눌렸을 때
    {
        this.GetComponent<ClickAudioClipUser>().PlayClickSound();

        StartButton.gameObject.SetActive(false);
        StartCoroutine(TryStartGame());
    }
  
    public float TouchedAnimationTime;
    IEnumerator TryStartGame()//서버 연결
    {
        yield return new WaitForSeconds(TouchedAnimationTime);
        TitleParticleSystem.SetActive(false);
 
        accountChecker.gameObject.SetActive(true);
        accountChecker.text = "서버 연결 시도 중";
        yield return StartCoroutine(GameManager.instance.TryConnectServer());
        accountChecker.gameObject.SetActive(false);
        if (GameManager.instance.OnServerConnect)
        {


       
            StartCoroutine(CheckAsset());
        }
        else
        {
            GameManager.instance.ActiveWarning("서버와의 연결이 원활하지 않습니다.\n싱글모드로 전환"
             , () =>
             {
                 GameManager.instance.ActiveSignleModeText();
                 GameManager.instance.singlemode = true;
                 StartCoroutine(CreateAccount());
             } );
         
            //GameManager.instance.ActiveWarning("서버와의 연결이 원활하지 않습니다.",
            //    () =>
            //    {
            //        LoadManager.instance.LoadTitle();

            //    });
        }
    }
   
    IEnumerator CheckAsset()
    {
        accountChecker.gameObject.SetActive(true);
        accountChecker.text = "리소스 확인 중..";
        yield return StartCoroutine(GameManager.instance.TryCheckAppVer());
        
        if (cache == null)
        {
       
            StartCoroutine(CreateAccount());
        }
        else
        {
       
            StartCoroutine(ConnectAccount(cache.UID));
        }
    }
    IEnumerator LoadGame()
    {



        yield return null;

        accountChecker.gameObject.SetActive(false);
        LoadManager.instance.loadMainMenu();
  
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataExecute;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System;
using network;
using TMPro;

public class GameManager : MonoBehaviour
{
  
    public UserAccountData Account;
    public GamePlayCache GamePlayCache;

    #region shop
    public List<ShopItem> CurrentShopList { get; set; }
    public IEnumerator SaveShopList(List<ShopItem> list,int shopid,  Action act,Action act2)
    {
       
        if (act != null) act.Invoke();
        ActiveLoadingImg();
        yield return new WaitForSeconds(0.1f);
        DeactiveLoadingImg();

        if (!OnError && act2 != null)
            act2.Invoke();

        ////act.Invoke();
        //ActiveLoadingImg();
        //yield return network.TrySaveShopList(shopid,list, Account.uid);
        //DeactiveLoadingImg();
        //if (!OnError)
        //    act2.Invoke();

    }
    public IEnumerator RequestShopList(int shopid,Action act)
    {
        if (CurrentShopList == null)
            CurrentShopList = new List<ShopItem>();

        CurrentShopList.Clear();
        ActiveLoadingImg();
        yield return new WaitForSeconds(0.1f);

        //TextAsset shopJsonAsset = Resources.Load<TextAsset>("Json/Shop/shop_basic");

        //ShopItemDictionary shopData = null;
        //if (shopJsonAsset != null)
        //{

        //    shopData = JsonUtility.FromJson<ShopItemDictionary>(shopJsonAsset.text);

        //}

        DataExecute.UserDataManager dataManager = new DataExecute.UserDataManager();

        // shop_basic.json을 읽어옴 (Assets/StreamingAssets/Shop/shop_basic.json 에 있어야 함)
        ShopItemDictionary shopData = dataManager.readShopCatalog("shop_basic");

        if (shopData != null && shopData.ItemDictionary.Count > 0)
        {
            Debug.Log("상점 JSON 로드 성공: shop_basic.json");
            // 첫 번째 묶음(List)에 있는 경험치 포션들을 가져옴
            foreach (ShopItem item in shopData.ItemDictionary[0].List)
            {
                CurrentShopList.Add(item);
            }
        }
        else
        {
            Debug.LogWarning("상점 JSON을 찾지 못했습니다. 경로를 확인하세요.");
        }

        DeactiveLoadingImg();

        if (!OnError && act != null)
            act.Invoke();

        //     if (CurrentShopList == null)
        //         CurrentShopList = new List<ShopItem>();

        //    ActiveLoadingImg();
        //yield return     network.TryRequestShopList(shopid, Account.uid);
        //     DeactiveLoadingImg();
        //     if(!OnError)
        //         act.Invoke();

    }
    #endregion
    public TextMeshProUGUI ChasingLOG;
    public void ActiveChasingLog(String s)
    {
        ChasingLOG.gameObject.SetActive(true);
        ChasingLOG.text = s;
    }
   
    #region GameManagerCanvas
    public GameObject LoadingImg;
    public GameObject SingleModeText;
    public GameObject grayBG;
    public GameObject invisibleBG;
    public bool OnError;
    public DescriptionPopUp ITemPopUp;
    public DescriptionPopUp ChrPopUP;
   public ItemDescriptionUI ItemDescription;
   
    public LVUpUI LVUpUI;
    public ErrorMessage error;
    public WarningMessage Warning;
    public void ActiveGrayBG()
    {
        //Debug.Log("GrayBG Active");
        grayBG.SetActive(true);
    }
    public void ActiveInvisibleBG()
    {
        invisibleBG.SetActive(true);
    }
    public void DeactiveInvisibleBG()
    {
        invisibleBG.SetActive(false);
    }
    public  void DeactiveGrayBG()
    {
        if (!LoadingImg.activeSelf &&
            !ItemDescription.gameObject.activeSelf && !error.gameObject.activeSelf &&
            !Warning.gameObject.activeSelf)
            grayBG.SetActive(false);
    }
    public void ActiveSignleModeText()
    {
        SingleModeText.SetActive(true);
    }
    public void ActiveChrPopUp(string name,Transform t)
    {
        ChrPopUP.ActivePopUp(name,t);
        ChrPopUP.gameObject.SetActive(true);
    }
    public void ActiveLVUpUI(int LV,int PastStamina,
        int CurrentStamina)
    {
        ActiveGrayBG();
        LVUpUI.ActiveLVUpUI(LV.ToString(),
            PastStamina.ToString(), CurrentStamina.ToString());
    }
    public void ActiveError(string s)
    {
        ActiveGrayBG();
        error.activemessage(s);
        OnError = true;
    }
    public void ActiveWarning(string s,Action a=null)
    {
        ActiveGrayBG();
        Warning.activemessage(s, a);
    }
    public void ActiveDescPopUp(string name,string desc,Transform t)
    {
        ITemPopUp.ActivePopUp(name,desc,t);
        ITemPopUp.gameObject.SetActive(true);
    }
  
    public void ActiveItemDescription(item i)
    {
        ActiveGrayBG();
        ItemDescription.ActiveItem(i);
        ItemDescription.gameObject.SetActive(true);
    }
    public void ActiveLoadingImg()
    {
        if (SceneManager.GetActiveScene().name != "Loading")
        {
            LoadingImg.SetActive(true);
            ActiveInvisibleBG();
        }
    }
    public void DeactiveLoadingImg()
    {

        LoadingImg.SetActive(false);
        DeactiveInvisibleBG();
    }
    #endregion

    #region setting
    
    public SettingData _settingdata;
    public AudioMixer audiomixer;
    public int GraphicSettingindex = 2;
    public void SaveSetting()
    {
        FIleIOManager.CreateSetting(_settingdata);
    }
    public void loadSetting()
    {
        _settingdata = FIleIOManager.loadSetting();
        if( _settingdata.BGMVolume!=0 ) 
        audiomixer.SetFloat("BG", Mathf.Log10(_settingdata.BGMVolume) * 10);
        else
            audiomixer.SetFloat("BG", -80);
        if( _settingdata.SEVolume!=0)
        audiomixer.SetFloat("SE", Mathf.Log10(_settingdata.SEVolume) * 10);
        else
            audiomixer.SetFloat("SE", -80);
        QualitySettings.SetQualityLevel(GraphicSettingindex, false);
    }
    public void ReWriteSetting(string section, string key, string value)
    {
        FIleIOManager.ReWriteSetting(section, key, value);
    }
    #endregion





    public static GameManager instance;
    private void Awake()
    {
        if(instance==null)
        instance = this;
        else if(instance!=this)
        {
            Destroy(this.gameObject);
        }

        network =new UnityNetWorkHandler( this.gameObject.GetComponent<NetworkManager>());
        //GamePlayCache= FIleIOManager.LoadGameplayCache();
        GamePlayCache = new GamePlayCache() { BasciGamePlayTutorialComplete1 = true ,BasciGamePlayTutorialComplete2=true,CharacterSortOption1=0,CharacterSortOption2=0};
        error.gameObject.SetActive(false);
      


        Account = null;

        DontDestroyOnLoad(this);
  
    }
    private void Start()
    {
        loadSetting();
    }
    #region network&load
    public string ServerMessage;
    public bool OnServerConnect;
    UnityNetWorkHandler network;
    public void DisConnectServer()
    {
        network.DisConnectServer();
        Account = null;
    }
   public IEnumerator TryCheckAppVer()
    {
        yield break;

        //List<string> bundleversion = new List<string>();
        //yield return StartCoroutine(LoadManager.instance.CheckBundleVersion("character", (s) => { bundleversion.Add(s); }));
        //yield return StartCoroutine(network.TryCheckApplication(bundleversion));

        //if (ServerMessage == "Asset Download Need")
        //{
        //    //yield return new WaitUntil(() => ServerMessage == "Asset Download Complete");
        //    yield return StartCoroutine(DownLoadManager.instance.DownLoadBundle());
        //}
    }
    public bool singlemode;
    public IEnumerator TryConnectServer()
    {
        GameManager.instance.OnServerConnect = false;
        singlemode = true;
        yield break;
        //GameManager.instance.OnServerConnect = false;
        //float time = 0;
        //while (true)
        //{
        //    if (time < 5 && !GameManager.instance.OnServerConnect)
        //    {

        //        time += Time.deltaTime;
        //        network.ConnectServer();

        //        yield return null;
        //    }

        //    else if(time>=6)
        //    {
        //        Debug.Log("연결 시간 초과 싱글모드 전환");
        //        singlemode = true;

        //        yield break;
        //    }
        //    else
        //    {

        //        yield break;
        //    }
        //}
    }
  
    public void Deleteaccountcorutine()
    {
        FIleIOManager.DeleteConnectionCache();
        ActiveWarning("클라이언트의 데이터가 초기화 되었습니다.\n" +
            "타이틀로 돌아갑니다.", () =>
            {
                Account = null;
                LoadManager.instance.LoadTitle();
            });
        
    }
    
    public IEnumerator createaccountcorutine(string username)//코루틴으로 만들어야하나?
    {

        ActiveLoadingImg();
    yield return StartCoroutine(network.TryCreateAccount(username));
        DeactiveLoadingImg();


        if (!OnError)
            FIleIOManager.MakeConnectionCache(Account);


    }
    public IEnumerator createaccountcorutineSingle(string username)
    {

        ActiveLoadingImg();
        yield return new WaitForSeconds(0.2f);
        Account= LoadManager.instance.loaddummyAccount();
        //Account.

      //  Account = new UserAccountData();
      //instance.  Account.username = username;
      //  instance.Account.StarChracter = "";
      //  instance.Account.uid = "singlemode";
      //  instance.Account.MaxStamina = 999;
      //  instance.Account.Stamina = 999;
      //  instance.Account.initializelist();

        DeactiveLoadingImg();




    }
    public IEnumerator ConnectAccountCorutine(string uid)//계정 연결 시도
    {
        ActiveLoadingImg();
        yield return new WaitForSeconds(0.2f); // 약간의 로딩 연출 (화면 전환용)

        // 3. 더미 계정 로드 시도
        if (LoadManager.instance.DummyAccount != null)
        {
            Account = LoadManager.instance.loaddummyAccount();
        }

        // 4. 더미 파일이 없거나 딕셔너리가 비어있어 UI가 고장나는 것을 방지 (하드코딩 주입)
        if (Account == null)
        {
            Account = new UserAccountData();
            Account.initializelist();
            Account.username = "포트폴리오";
            Account.MaxStamina = 999;
            Account.Stamina = 999;
        }

        if (Account.get_CharacterList() == null || Account.get_CharacterList().Count == 0)
        {
            Account.initializelist(); // 딕셔너리 널 방지 초기화

            // ★ 중요: c01, c02 부분은 실제 존재하는 캐릭터 ID(프리팹/DB 이름)로 바꾸셔야 UI에 뜹니다!
            Account.set_Character(new PlayerData() { id = "c01", });
            Account.set_Character(new PlayerData() { id = "c02", });
        }

        Account.uid = uid;
        FIleIOManager.MakeConnectionCache(Account); // 로컬 저장 캐시 갱신

        DeactiveLoadingImg();
        yield break; // 원래 있던 network.TryConnectAccount 패킷 전송 로직 완전 무시
        //ActiveLoadingImg();
        //yield return StartCoroutine(network.TryConnectAccount(uid));
        //DeactiveLoadingImg();
        //if (!OnError&&Account.uid!="")
        //{
        //    FIleIOManager.MakeConnectionCache(Account);//캐시 갱신
        //    Debug.Log("회복전 스테미나" + Account.Stamina + "타이머" + Account.StaminaTimer);
        //    yield return StartCoroutine(saveAccountcorutine(() =>
        //    {
        //        string currentDate = DateTime.Now.ToString("yyyyMMddhhmmss");
        //        Debug.Log(currentDate + " " + Account.LastestConnect);
        //        var a = calculatebydate(currentDate, Account.LastestConnect, Account.StaminaTimer, Account.Stamina, Account.MaxStamina);
        //        Account.LastestConnect = currentDate;
        //        Account.StaminaTimer = a.Item1;
        //        Account.Stamina = a.Item2;
        //        Debug.Log("계산 후 스테미나" + Account.Stamina + "타이머" + Account.StaminaTimer);
        //    }));
        //}
    }
    public IEnumerator FakeConnectAccountCorutine(string uid)//서버 고장 문제로 싱글 전환 가짜 계정 생성
    {
        ActiveLoadingImg();
        yield return new WaitForSeconds(0.2f); // 연출용 대기

        // 1. UserDataManager를 통해 로컬에 만들어둔 완벽한 템플릿(AccountTemplet.json)을 불러옵니다.
        DataExecute.UserDataManager dataManager = new DataExecute.UserDataManager();
        UserAccountDataNetworkMember templateMember = dataManager.GetUserAccountTemplet();

        if (Account == null)
        {
            Account = new UserAccountData();
        }

        // 2. 서버에서 받은 것처럼 템플릿 데이터를 계정에 덮어씌웁니다.
        Account.GetNetworkMember(templateMember);

        // 3. UI 구동을 위해 UID와 골드/스테미나를 넉넉하게 세팅해 줍니다.
        Account.uid = uid;
        Account.username = "포트폴리오(싱글)";
        Account.MaxStamina = 999;
        Account.Stamina = 999;
        Account.SetMoney(999999);
   

        // 4. 로컬 캐시 갱신 후 화면 전환
        FIleIOManager.MakeConnectionCache(Account);

        DeactiveLoadingImg();
        yield break;
    }
    public IEnumerator saveAccountcorutine(Action act)//계정 저장
    {
        ActiveLoadingImg();
        yield return new WaitForSeconds(0.1f);
        // 서버 저장 대신 로컬 캐시만 생성 후 즉시 콜백 실행
        //FIleIOManager.MakeConnectionCache(Account);
        DeactiveLoadingImg();
        if (act != null) act.Invoke();
        //if (act != null) act.Invoke();
        //ActiveLoadingImg();
        //yield return StartCoroutine(network.TrySaveAccount(Account,act));
        //DeactiveLoadingImg();


    }
   
    #region calculate stamina
    static Tuple<float, int> calculatebydate(string current, string Lastest, float timer, int stamina, int Maxstamina)
    {
     
        //DateTime.parse=yyyymmddhhmmss 형태에 사용시 오류 발생->DateTime.ParseExact 사용
       
        if (stamina < Maxstamina)
        {
            TimeSpan ts = DateTime.ParseExact(current, "yyyyMMddHHmmss", null) - DateTime.ParseExact(Lastest, "yyyyMMddHHmmss", null);
            int second = Mathf.RoundToInt((float)ts.TotalSeconds);

            int AddStamina = second / global.StaminaCreateTime;
            stamina += AddStamina;
      
            if (stamina >= Maxstamina)
            {

                stamina = Maxstamina;
                timer = global.StaminaCreateTime;
            }
            else
            {
                timer -= second % global.StaminaCreateTime;
                if (timer <= 0)
                {

                    if (++stamina == Maxstamina)
                    {
                        timer = global.StaminaCreateTime;
                    }
                    else
                    {
                        timer = global.StaminaCreateTime + timer;
                    }
                }
            }
        }
     
       
       
        return new Tuple<float, int>(timer, stamina);
       
    }

    #endregion


    #endregion
    private void FixedUpdate()
    {
        if (Account != null) 
            Account.AccountUpdate();

        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

    }

}

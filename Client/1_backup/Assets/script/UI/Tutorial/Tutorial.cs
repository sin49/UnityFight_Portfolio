using menu;
using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : ScriptMessageBox
{
    public static Tutorial Instance;

    int ScriptNumber;


    private void Awake()
    {
       
        Instance = this;
        yellowframe.gameObject.SetActive(false);
        foreach(GameObject fence in UIFence)
        {
            fence.gameObject.SetActive(false);
        }
    }
   
    private void OnEnable()
    {
        Menu.instance.tutorialUI = this;
        MessageScript = TextScriptReader.TextFileReader("Tutorial", "txt");
    
        ScriptNumber = int.Parse(TextScriptReader.SliceTextScript(MessageScript, "Text Length"));
        MessageBoxOutPutComplete = true;


        ActiveUIFence(0);
    
        StartCoroutine(Tutorial1());
    }

    public RectTransform yellowframe;

    
    public override void MessageBoxOutPutCompleteCheck()
    {
        base.MessageBoxOutPutCompleteCheck();
        //index++;
        //Debug.Log("index가 올랐습니다:" + index);
        //textscript = TextScriptReader.SliceTextScript(MessageScript, index.ToString());
    }
    void ActiveUIFence(int n)
    {
        if (ActivedUIFence != null && UIFence[n]!=ActivedUIFence)
        {
            ActivedUIFence.SetActive(false);
            ActivedUIFence = null;
        }
        UIFence[n].SetActive(true);
        ActivedUIFence = UIFence[n];
    }
    void activeYeloowframe(int n)
    {
        //764.8704,-469
        yellowframe.transform.localPosition = yellowFramePosition[n];
        yellowframe.sizeDelta = yellowframeWH[n];
        Debug.Log("프레임을 이동시켰습니다:" + yellowFramePosition[n]);
        yellowframe.gameObject.SetActive(true);
    }
    CharactersSelectUI selectui;
    CharacterUpgradeUI upgradeui;
    //하드 코딩
    IEnumerator Tutorial1()
    {

        //Main에서 부터 게임 플레이까지
        //StartCoroutine(OutputMessageBox(textscript));
        while (index <= ScriptNumber)
        {
         
            switch (index)//하드 코딩으로 화살표 활성화&버튼 통제를 실행한다
            {//버튼 통제 방법:1.이미지로 가림막을 만든다-만들 때만 귀찮고 코드로 버튼 통제 안해도 됨
             //2.코드를 통해 버튼의 onclick,interactable을 제어한다-버튼눌려야 되는 순간마다 
             //새로 제어해야함 버튼을 찿는 과정에 메모리도 잡아먹을 것으로 추정
             //결론: 1번이 무식하지만 압도적으로 좋아보임
             //가림막은 투명으로 만들고 강조 프레임으로 표시
             //0-0클리어 보상으로 경험치 아이템 추가
             //캐릭터 강화 부분 너무 귀찮게 하지말고 레벨 부분만 처리
             //강화 부분에서 경험치로 강화하는 부분 추가
                case 0:
                    UpdateMessageBox();
                    break;
                case 2:
                    Debug.Log("case 2에 진입함");
                    activeYeloowframe(0);
                    ActiveUIFence(1);
                    yield return new WaitUntil(()=>Menu.instance.StageUI.transform.parent.gameObject.activeSelf);
                   
                        UpdateMessageBox();
             

                    break;
                case 3://main 클릭
                    activeYeloowframe(1);
                    ActiveUIFence(2);
                    yield return new WaitUntil(
                        () => Menu.instance.StageUI.MainStageSelectedUI.activeSelf);
                   
              

                        UpdateMessageBox();

                    break;
                case 4://0-0 클릭
                    activeYeloowframe(2);
                    ActiveUIFence(3);
                    yield return new WaitUntil(
                        () => Menu.instance.StageUI.infoui.gameObject.activeSelf);
                

                        UpdateMessageBox();
                    

                    break;
                    case 6:
                    activeYeloowframe(3);
                    if (Input.GetMouseButton(0) && !LoadManager.instance.onload/*|| Input.touchCount > 0*/)
                    {
                        UpdateMessageBox();
                    }
                    break;
                //case 8:

                //    break;
                case 9://시작 버튼
                    activeYeloowframe(4);
                    ActiveUIFence(4);
                    yield return new WaitUntil(
                        () => Menu.instance.StageUI.CharacterSelectUI.activeSelf);
                    selectui = Menu.instance.StageUI.CharacterSelectUI.GetComponent<CharactersSelectUI>();
                 

                        UpdateMessageBox();

                    break;
                case 11://캐릭터 리스트 첫번째 캐릭터,아니면 두번째 캐릭터 클릭
                    activeYeloowframe(5);
                    ActiveUIFence(5);
                    yield return new WaitUntil(
                        () => selectui.SelectedPlayerUI[0].Data!=null);
                    
                        UpdateMessageBox();

                    
                    break;
                case 13://시작 버튼
                    activeYeloowframe(6);
                    ActiveUIFence(6);
                    DontDestroyOnLoad(this);
                    yield return new WaitUntil(
                        () => SceneManager.GetActiveScene().name=="Loading");
                    Messagebox.transform.parent.gameObject.SetActive(false);
                    yellowframe.gameObject.SetActive(false);
                    yield return new WaitUntil(
                        () => SceneManager.GetActiveScene().name == "Stage");
                    Messagebox.transform.parent.gameObject.SetActive(true);

                  

                        UpdateMessageBox();

                    break;
                    //방법 1:인게임용 튜토리얼 따로 만들기
                    //방법 2:Don'tdestroyOnload로 한 곳에 만들기
                case 14:
                    Time.timeScale = 0;
                    if (Input.GetMouseButton(0) && !LoadManager.instance.onload/*|| Input.touchCount > 0*/)
                    {
                        ActiveUIFence(0);

                        UpdateMessageBox();
                    }
                    break;
                    case 15:

                    if (Input.GetMouseButton(0) && !LoadManager.instance.onload/*|| Input.touchCount > 0*/)
                    {
                        activeYeloowframe(7);
                        ActiveUIFence(0);

                        UpdateMessageBox();
                    }
                    break; 
                case 16:
                    if (Input.GetMouseButton(0) && !LoadManager.instance.onload/*|| Input.touchCount > 0*/)
                    {
                        ActiveUIFence(0);
                        yellowframe.gameObject.SetActive(false);
                        Messagebox.transform.parent.gameObject.SetActive(false);
                        Time.timeScale = 1;
                    }
                    yield return new WaitUntil(() => Stage._stage.PlayerCreated[0].LB_gauge
                   > 0);
                    UpdateMessageBox();
                    Time.timeScale = 0;
                    Messagebox.transform.parent.gameObject.SetActive(true);
                    activeYeloowframe(8);
                    break;
                case 17:
                    if (Input.GetMouseButton(0) && !LoadManager.instance.onload/*|| Input.touchCount > 0*/)
                    {
                        ActiveUIFence(0);
                        
                        UpdateMessageBox();
                        Stage._stage.allPlayerSpFull();
                    }
                 
                    break;
                case 18://특수기 사용

                    //Time.timeScale = 1;
                    Time.timeScale = 1;
                    yield return new WaitForSeconds(0.1f);

                    yield return new WaitUntil(() => Stage._stage.PlayerCreated[0].LB_gauge
                    == Stage._stage.PlayerCreated[0].LB_gauge_max);
                    //UpdateMessageBox();
                    activeYeloowframe(8);
                    ActiveUIFence(7);
                    Messagebox.transform.parent.gameObject.SetActive(true);
                    Time.timeScale=0;
                    yield return new WaitUntil(()=>
                   EventSystem.current.currentSelectedGameObject!=null && EventSystem.current.currentSelectedGameObject.GetComponent<ChracterGameIconUI>()!=null
                     );//버튼 누른거로 확인 1. eventsystem으로 확인2.캐릭터 능력치로 확인
                    Time.timeScale = 1;
                    Messagebox.transform.parent.gameObject.SetActive(false);
                    yellowframe.gameObject.SetActive(false);
                    UpdateMessageBox();
                    break;
                case 19:
                    yield return new WaitUntil(() => Stage._stage.WinCanvas.transform.parent.gameObject.activeSelf);
                    Messagebox.transform.parent.gameObject.SetActive(true);
                    yellowframe.gameObject.SetActive(false);
                    if (Input.GetMouseButton(0) && !LoadManager.instance.onload/*|| Input.touchCount > 0*/)
                    {

                        UpdateMessageBox();
                    }

                    
                    break;
                case 21://스테이지 완료
                    activeYeloowframe(9);
                    ActiveUIFence(8);

                    yield return new WaitUntil(
                         () => SceneManager.GetActiveScene().name == "Loading");
                    GameManager.instance.GamePlayCache.BasciGamePlayTutorialComplete1 = true;
                    FIleIOManager.MakeGamePlayCache(GameManager.instance.GamePlayCache);
                    Messagebox.transform.parent.gameObject.SetActive(false);
                    yellowframe.transform.gameObject.SetActive(false);
                    yield return new WaitUntil(
                        () => SceneManager.GetActiveScene().name == "MainMenu");

                    Messagebox.transform.parent.gameObject.SetActive(true);
                    yellowframe.transform.gameObject.SetActive(true);
                    UpdateMessageBox();

                    
                    break;
                case 22://캐릭터 버튼
                    activeYeloowframe(10);
                    ActiveUIFence(9);
                    yield return new WaitUntil(() => Menu.instance.CharacterUI.transform.parent.gameObject.activeSelf);
                   
                        UpdateMessageBox();

                    
                    break;

                case 23://캐릭터 리스트 첫번째 아이템
                    activeYeloowframe(11);
                    ActiveUIFence(5);
                    yield return new WaitUntil(() => Menu.instance.CharacterUI.CharacterUpgrade.activeSelf);
                    upgradeui = Menu.instance.CharacterUI.CharacterUpgrade.GetComponent<CharacterUpgradeUI>();
                   
                        UpdateMessageBox();

                    
                    break;
                case 25://레벨 버튼
                    activeYeloowframe(12);
                    ActiveUIFence(10);
                    yield return new WaitUntil(()=>upgradeui.LVUpgradeUI.gameObject.activeSelf);
                   
                        UpdateMessageBox();
                    yellowframe.gameObject.SetActive(false);
                    
                    break;
                case 27://닫기
                    activeYeloowframe(13);
                    ActiveUIFence(11);
                    yield return new WaitUntil(() => !upgradeui.LVUpgradeUI.gameObject.activeSelf);
                   
                        UpdateMessageBox();

                    
                    break;
                  
                case 28://메인 버튼
                    activeYeloowframe(14);
                    ActiveUIFence(12);
                    yield return new WaitUntil(() => Menu.instance.MainUI.gameObject.activeSelf);
                   
                        UpdateMessageBox();

                    
                    break;
                case 29:
                    if (Input.GetMouseButton(0) && !LoadManager.instance.onload/*|| Input.touchCount > 0*/)
                    {
                        this.gameObject.SetActive(false);
                        GameManager.instance.GamePlayCache.BasciGamePlayTutorialComplete2 = true;
                        FIleIOManager.MakeGamePlayCache(GameManager.instance.GamePlayCache);
                    }
                    break;
                        default:
                    if(yellowframe.gameObject.activeSelf)
                        yellowframe.gameObject.SetActive(false);
                    ActiveUIFence(0);
                    if (Input.GetMouseButton(0) && !LoadManager.instance.onload/*|| Input.touchCount > 0*/)
                    {
                    
                        UpdateMessageBox();
                        yield return new WaitForSecondsRealtime(0.5f);
                    }
                    break;
            }

            yield return null;
        }
        GameManager.instance.GamePlayCache.BasciGamePlayTutorialComplete2 = true;
        FIleIOManager.MakeGamePlayCache(GameManager.instance.GamePlayCache);
        this.gameObject.SetActive(false);

    }
    [Header("position")]
    public List<Vector2> yellowFramePosition;
    public List<Vector2> yellowframeWH;
    [Header("UIfence")]
    public List<GameObject> UIFence;
    GameObject ActivedUIFence;
  
}

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;


public class Stage : MonoBehaviour, Character_observer,StageBroadCaster
{






    public GameObject PullingChrObjList;
    public Stack<GameCharacter> PullingChrList = new Stack<GameCharacter>();
    public void AddPullingChrList(GameCharacter character)
    {
        character.transform.SetParent(Stage._stage.PullingChrObjList.transform);
    
        PullingChrList.Push(character);
    }
    public GameCharacter GetCharacterFromPullingList(string uid)
    {
        List<GameCharacter> tmplist=new List<GameCharacter> ();
        for(int n = 0; n < PullingChrList.Count; n++)
        {
            var a=PullingChrList.Pop ();
            if (a.information.id == uid)
            {
                tmplist.Reverse();
                foreach(GameCharacter obj in tmplist)
                    PullingChrList.Push(obj);
                a.name = a.name + "+";
                a.gameObject.SetActive(true);
                return a;
            }
            else
                tmplist.Add(a);
        }
        tmplist.Reverse();
        foreach (GameCharacter obj in tmplist)
            PullingChrList.Push(obj);
        return null;

    }
    public static Stage _stage = null;
    private void Awake()
    {
        _stage = this;
       
        BackGroundAudioPlayer.instance.PlaystageBG();
        //if (icon_plate != null && chr_icon_ui != null)
        //{
        //    for (int i = 0; i < icon_ui_list.Length; i++)
        //    {
        //        GameObject ui = Instantiate(chr_icon_ui);
        //        ui.transform.SetParent(icon_plate.transform);
        //        icon_ui_list[i] = ui.GetComponent<CharacterGameStatusUI>();
        //    }
        //}
        WinCanvas.transform.parent.gameObject.SetActive(false);
        LoseCanvas.SetActive(false);
    }



    #region UI
    //icon
    //public GameObject icon_plate;
    //public GameObject chr_icon_ui;
   public CharacterGameStatusUI[] icon_ui_list = new CharacterGameStatusUI[5];
    public Canvas slider_Canvas;
    public Canvas font_Canvas;
    public Canvas Pause_Canvas;
    public GameObject CHRslider;
    List<character_slider> CHRslider_create = new List<character_slider>();
    public GameObject Damage_font_Enemy;
    public GameObject Damage_font_Player;
    List<GameObject> Damage_font_pulling_Enemy = new List<GameObject>();
    List<GameObject> Damage_font_pulling_Player = new List<GameObject>();
    void create_slider(GameCharacter gc)
    {
        character_slider h = Instantiate(CHRslider, slider_Canvas.transform).GetComponent<character_slider>();
        CHRslider_create.Add(h);
        gc.AddObserver(h);

    }
    void pulling_HPbar(GameCharacter gc)
    {
        foreach (character_slider a in CHRslider_create)
        {
            if (a.Character_transform == gc.gameObject)
            {
                a.update_information(gc.current_hp, gc);
                a.gameObject.SetActive(true);
            }
        }
    }
    public Damage_font pulling_damage_font(Team T)
    {
        GameObject obj = null;
        List<GameObject> Damage_Font_List;
        GameObject damage_font;
        if (T == Team.Enemy)
        {
            damage_font = Damage_font_Enemy;
            Damage_Font_List = Damage_font_pulling_Enemy;
        }
        else
        {
            damage_font = Damage_font_Player;
            Damage_Font_List = Damage_font_pulling_Player;
        }
        for (int i = 0; i < Damage_Font_List.Count; i++)
        {
            if (!Damage_Font_List[i].activeSelf)
            {
                obj = Damage_Font_List[i];
                obj.SetActive(true);
                break;
            }
        }
        if (obj == null)
        {
            obj = Instantiate(damage_font, font_Canvas.transform);
            Damage_Font_List.Add(obj);
        }
        return obj.GetComponent<Damage_font>();
    }
    #endregion


    #region 스테이지 진행
    public float left_time;
    float MaxLeftTime = 180;
    public GameObject UICanvas;
    public GameObject StartText;
    public GameClearUI WinCanvas;
    public TextMeshProUGUI ClearLVText;
    public GameObject LoseCanvas;
    public StageInformation info;
    int PlayerDieCount = 0;
    PlayerData[] PlayerDatas;
    public TextMeshProUGUI RoundText;
    public int current_round_number = 0;
    public float WaveChangeWaitTIme = 3.5f;
    public bool WaveCleared;
    public void ReTryStage()
    {
        if (GameManager.instance.Account.Stamina >= info.StaminaNeed)
            LoadManager.instance.ReLoadGame(info, Enemies, Players, PlayerDatas);
        else
        {

            BackStage();
        }
    }
    public void BackStage()
    {
        LoadManager.instance.BackMainMenu(info);
    }
    public void ResetStage(StageInformation info,List<GameObject> characters, List<GameObject> Players, PlayerData[] playerdata)
    {
        this.Enemies = characters;
        this.info = info;
        this.Players = Players;
        PlayerDatas = playerdata;

        initializeStage();
        StartCoroutine(StageStarter());
    }

    public void setStage(StageInformation info, List<GameObject> charcters, PlayerData[] playerdata, int playernumber,GameObject Terrain)
    {
        this.Players = new List<GameObject>();
        PlayerDatas = playerdata;

        for (int i = 0; i < playernumber; i++)
        {

            Players.Add(charcters[0]);
            charcters.RemoveAt(0);
        }
        this.Enemies = charcters;
        this.info = info;
        if (Terrain != null)
        {
           this.Terrain = Terrain;
        }

        initializeStage();
        StartCoroutine(StageStarter());
    }

    public void setStage(StageInformation info, List<GameObject> charcters, PlayerData[] playerdata,int playernumber)
    {
        this.Players = new List<GameObject>();
        PlayerDatas = playerdata;
  
        for (int i = 0; i < playernumber; i++)
        {

            Players.Add(charcters[0]);
            charcters.RemoveAt(0);
        }
        this.Enemies = charcters;
        this.info = info;


        initializeStage();
        StartCoroutine(StageStarter());
    }
    void initializeStage()
    {
        StartText.gameObject.SetActive(true);
        left_time = MaxLeftTime;
        current_round_number = 1;
        Pause_Canvas.gameObject.SetActive(true);
        initailizeField();
        
        
        set_CenterPoint_viewport();

    }

    bool active_player()
    {
        foreach (GameCharacter player in PlayerCreated)
        {
            player.active_obj();
        }
        return true;
    }
    bool active_enemy()
    {
        foreach (GameCharacter enemy in EnemyCreated)
        {
            enemy.active_obj();
        }
        return true;
    }
    IEnumerator StageStarter()
    {
        yield return create_player();
        yield return SpawnEnemyByRound(current_round_number);
        UICanvas.SetActive(true);
        StartText.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.25f);
        notifyObserver();
        yield return new WaitForSeconds(0.75f);
        active_player();
        active_enemy();
        StartCoroutine(StagePlaying());
    }
    void DeActiveCharacter()
    {
        foreach (GameCharacter player in PlayerCreated)
        {
            player.deactive_chr();
        }
        foreach (GameCharacter enemy in EnemyCreated)
        {
            enemy.deactive_chr();
        }
    }
    IEnumerator SpawnEnemyByRound(int n)
    {
        yield return create_enemy_List(n);
        yield return create_enemy();
    }
    IEnumerator StagePlaying()
    {
        while (true)
        {
            yield return null;
            left_time -= Time.deltaTime;
            if (left_time <= 0)
            {
                left_time = 0;
                break;
            }
            //set_CenterPoint_viewport();
            TerrainGenerate();
            if (EnemyCreated.Count == 0)
            {
                WaveCleared = true;
                if (current_round_number == info.waveNumber)
                {
                    DeActiveCharacter();
                    UICanvas.SetActive(false);
                    font_Canvas.gameObject.SetActive(false);
                    slider_Canvas.gameObject.SetActive(false);
                    Pause_Canvas.gameObject.SetActive(false);
                    StageClear();
                    yield break;
                }
                else
                {
                    current_round_number++;
                    notifyObserver();
                   
                    RoundText.text = "Round " + current_round_number + "/" + info.waveNumber;
                    RoundText.gameObject.SetActive(true);
                    yield return new WaitForSeconds(WaveChangeWaitTIme);
                    WaveCleared = false;
               
                }

                yield return StartCoroutine(SpawnEnemyByRound(current_round_number));
                active_enemy();

            }
            else if (PlayerCreated.Count == 0)
            {
                DeActiveCharacter();
                UICanvas.SetActive(false);
                font_Canvas.gameObject.SetActive(false);
                slider_Canvas.gameObject.SetActive(false);
                Pause_Canvas.gameObject.SetActive(false);
                LoseCanvas.SetActive(true);
                yield break;
            }

        }
        StageClear();
    }
    StageMission stagemisionClear(StageMission s)
    {
        switch (s.mission)
        {
            case StageMissionEnum.clear:
                s.Clear = true;
                break;
            case StageMissionEnum.minLimit_120:
                if (MaxLeftTime - left_time < 120)
                    s.Clear = true;
                break;
            case StageMissionEnum.PlayerDeath_0:
                if (PlayerDieCount == 0)
                    s.Clear = true;
                
                break;
        }
        return s;
    }
    void StageClear()
    {
        Debug.Log("라운드 종료");

        font_Canvas.gameObject.SetActive(false);
        slider_Canvas.gameObject.SetActive(false);
        DeActiveCharacter();
        ClearLVText.text = "";
        var a = GameManager.instance.Account.get_Stage(info.ID).stagemission;

        List<item> litemist = new List<item>();
       
        foreach (StageReward reward in info.rewardlist)
        {
            float rand = Random.Range(0, 1);
            if (rand < reward.per)
            {

                litemist.Add(reward.item);
                Debug.Log("Reward+=" + reward.item.itemid);
            }
        }
        WinCanvas.getStageReward(litemist);
        WinCanvas.getPlayerData(PlayerDatas, info.StaminaNeed * 20);
        //코루틴으로 바꾸고 yield return으로 처리 아니면 다 집어 넣기
        StartCoroutine(GameManager.instance.saveAccountcorutine(() =>
        {
           
            for (int n = 0; n < PlayerDatas.Length; n++)
            {
                
                    PlayerDatas[n].getEXP(info.StaminaNeed * 20);

                    GameManager.instance.Account.set_Character(PlayerDatas[n]);
                
            }
            GameManager.instance.Account.Stamina -= info.StaminaNeed;
            GameManager.instance.Account.getAccountEXP(info.StaminaNeed);
            foreach (item i in litemist) {
                GameManager.instance.Account.set_item(i);
            }
            for (int n = 0; n < a.Count; n++)
            {
                a[n].Clear = true;
                if (a[n].Clear)
                {
                    ClearLVText.text += "★";
                    if (ClearLVText.text.Length == 1)
                        ClearLVText.text += "\n";
                }
            }

            string currentID = info.ID;
            string[] split = currentID.Split('-'); // "Main0-0"을 "Main0"과 "0"으로 분리
            if (split.Length == 2)
            {
                if (int.TryParse(split[1], out int stageNum))
                {
                    string nextStageID = split[0] + "-" + (stageNum + 1); // "Main0-1" 생성
                    var nextStage = GameManager.instance.Account.get_Stage(nextStageID);
                    if (nextStage != null)
                    {
                        nextStage.stagelock = false; // 다음 스테이지 잠금 해제
                    }
                }
            }




            WinCanvas.transform.parent.gameObject.SetActive(true);
        }));


     
            

       

    }
    #endregion

    #region 이펙트 관리

    #endregion









    #region 캐릭터 생성

    List<GameObject> EnemySpawn;
    public spawn_point Player_spawn_point;
    public spawn_point Enemy_spawn_point;
     List<GameObject> Enemies;
 
     List<GameObject> Players;


    public List<GameCharacter> EnemyCreated = new List<GameCharacter>();

    public List<GameCharacter> PlayerCreated = new List<GameCharacter>();


    bool create_player()
    {
        

        for (int n = 0; n < Players.Count; n++)
        {
            //set_player_spawn_position();
            GameCharacter chr =Instantiate(Players[n]).GetComponent<GameCharacter>();
            PlayerCreated.Add(chr);
            Player_spawn_point.spawn(chr);
            chr.gameObject.layer = 6;
            chr.T = Team.Player;
            var a = icon_ui_list[4 - n];
            //pulling_HPbar(chr);
            chr.AddObserver(this);
            create_slider(chr);

            a.enable_icon_ui(chr);
            chr.AddObserver(a);

        }
        return true;
     }
    bool create_enemy_List(int roundnumber)
    {
        if (EnemySpawn == null)
            EnemySpawn = new List<GameObject>();
        else
            EnemySpawn.Clear();

        for (int i = 0; i < info.spawn.Count; i++)
        {
            if (info.spawn[i].round != roundnumber)
                continue;
            for (int n = 0; n < info.spawn[i].number; n++)
            {
                EnemySpawn.Add(Enemies[info.spawn[i].index]);
            }
            break;
        }
        return true;
    }
    bool create_enemy( )
    {
        for (int n = 0; n < EnemySpawn.Count; n++)
        {
            //set_enemy_spawn_position();

            GameCharacter chr = GetCharacterFromPullingList(EnemySpawn[n].GetComponent<GameCharacter>().information.id) ?? Instantiate(EnemySpawn[n]).GetComponent<GameCharacter>();
            
            chr.T = Team.Enemy;
            chr.gameObject.layer = 7;
         
            chr.set_pulling();
            EnemyCreated.Add(chr);
            Enemy_spawn_point.spawn(chr);

            //pulling_HPbar(chr);
            chr.AddObserver(this);
            create_slider(chr);  
        }
        return true;
    }

    #endregion
    #region 화면
    [UnityEngine.Range(0.5f, 1)]
    public float set_spawn_position_meter_by_viewport;
    public float viewport_snab_meter_z;
    public Camera GameCamera;
    public Vector3 screenCenter; 
    public Vector3 viewportCenter;
   public void set_CenterPoint_viewport()
    {
        viewportCenter = new Vector3(0.5f, 0.5f, 10 + (1.0f / Mathf.Tan(GameCamera.transform.rotation.x)));
        screenCenter = GameCamera.ViewportToWorldPoint(viewportCenter);
    }
  
    #endregion

    #region 테라인 생성
    public GameObject current_Field;
    GameObject Terrain;
    GameObject next_Field;
    float Field_Length;
    List<GameObject> Field_pulling = new List<GameObject>();
    public Transform FieldTransform;
    void initailizeField()
    {
        if (Terrain != null)
        {
            //current_Field.gameObject.SetActive(false);
            //current_Field = null;
            current_Field = Instantiate( Terrain);
            
          
            current_Field.gameObject.SetActive(true);
        }
             
        Terrain t = current_Field.GetComponent<Terrain>();
        Field_Length = t.terrainData.size.z;
        if (!Field_pulling.Contains(current_Field)) 
            Field_pulling.Add(current_Field);
       
            current_Field.transform.position = new Vector3(-20,0,-25);

        next_Field = null;
    }
    void TerrainGenerate()
    {

        if (next_Field != null && Stage._stage.screenCenter.z >= next_Field.transform.position.z + next_Field.transform.position.z / 4)
        {
           
            current_Field.SetActive(false);
            current_Field = next_Field;
            next_Field = null;
        }
        if (next_Field == null && Stage._stage.screenCenter.z >= current_Field.transform.position.z)
        {
            (next_Field = pulling_Field()).transform.position = current_Field.transform.position + Vector3.forward * Field_Length;
        }
    }
        
    GameObject pulling_Field()
        {
            foreach (GameObject obj in Field_pulling)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }
            var NewField = Instantiate(current_Field, FieldTransform);
            Field_pulling.Add(NewField);
            return NewField;
        }
    

        #endregion



     



    
   
    #region 옵저빙
    public void update_information(int a, GameCharacter character)
    {
        
        if (a > 0)
            return;
   
        switch (character.T)
        {
            case Team.Player:
                PlayerCreated.Remove(character);

                PlayerDieCount++;
               
                break;
            case Team.Enemy:
                EnemyCreated.Remove(character);
             

                break;
        }
        
    }
    List<StageObserver> Observers;
    public void AddObserver(StageObserver s)
    {
        if (Observers == null)
            Observers = new List<StageObserver>();
        Observers.Add(s);
    }

    public void DeleteObserver(StageObserver s)
    {
        Observers.Remove(s);
    }

    public void notifyObserver()
    {
        if (Observers == null)
            Observers = new List<StageObserver>();
        foreach (StageObserver observer in Observers)
        {
            observer.UpdateInformation(current_round_number);
        }
    }
    #endregion

    #region DebugMode
    public void allCharacterRecoverStart()
    {
      
        StartCoroutine(allCharacterRecoverCoroutine());
    }
   public void allEnemyKill()
    {
        for(int n=EnemyCreated.Count-1;n>=0; n--)
        {
            EnemyCreated[n].execute_Hit_handler(9999);
        }
       
    }
    public void allPlayerSpFull()
    {
        foreach (GameCharacter player in PlayerCreated)
        {
            player.gain_LBgauge(100) ;
        }
    }
    public void allPlayerSpRecover(float f)
    {
        foreach (GameCharacter player in PlayerCreated)
        {
            player.gain_LBgauge(f);
        }
    }
    public void allPlayerKill()
    {
        foreach (GameCharacter player in PlayerCreated)
        {
            player.execute_Hit_handler(9999); ;
        }
    }
    IEnumerator allCharacterRecoverCoroutine()
    {
        while (true)
        {
            yield return null;
            foreach(GameCharacter player in PlayerCreated)
            {
                player.execute_Heal_handler(player.MAX_hp);
            }
            foreach(GameCharacter Enemy in EnemyCreated)
            {
                Enemy.execute_Heal_handler(Enemy.MAX_hp);
            }
        }
    }
    #endregion


}

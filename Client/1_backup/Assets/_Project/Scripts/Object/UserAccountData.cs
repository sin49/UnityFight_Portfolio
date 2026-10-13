using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;


[Serializable]
public class UserAccountData
{
    public void initializelist()
    {
        Characterlist = new Dictionary<string, PlayerData>
        {
            { "1", new PlayerData() }
        };
        Clist = new List<PlayerData> {{new PlayerData() } 
        
        };
        Stagelist = new Dictionary<string, StageData>();
        itemlist = new Dictionary<string, item>();
    }
   

  
    [SerializeField]
    string _uid;
    public string uid { get { return _uid; } set { _uid = value; } }
    public string username;
    [SerializeField]
    int _accountlv;
    public int accountlv { get { return _accountlv; } set { _accountlv = value; } }
    [SerializeField]
    int _Max_Exp = 10;
    public int Max_Exp { get { return _Max_Exp; } set { _Max_Exp = value; } }
    public void getAccountEXP(int n)
    {
        bool LVUP=false;
        Exp += n;
        int tmp = MaxStamina;
        while (Exp >= Max_Exp)
        {
            LVUP = true;
            Exp -= Max_Exp;
            accountlv++;
            if (MaxStamina > 100)
                MaxStamina++;
            Stamina += MaxStamina;
            if (Max_Exp < 600)
                Max_Exp += 10 * (accountlv - 1);


        }
        if (LVUP)
        {
            GameManager.instance.ActiveLVUpUI(accountlv, tmp, MaxStamina);
        }
    }
    [SerializeField]
    int _Exp;
    public int Exp { get { return _Exp; } set { _Exp = value; } }
    public string LastestConnect;

    public string MaxStageClearData;

    public string StarChracter;
    public GameObject StarCharacterModel { get; set; }


    #region itemlist

  
    [SerializeField]
    Dictionary<string, item> itemlist=new Dictionary<string, item>();





    public void SetMoney(int n)
    {
        if (itemlist.ContainsKey("I01")) itemlist["I01"].Amount += n;
        else
        {
            itemlist.Add("I01", new item() { itemid = "I01", Amount = n });

        }
    }
    public item GetMoney()
    {
        item money; if (itemlist.TryGetValue("I01", out money)) return money;
        else
        {
            SetMoney(0);
            return itemlist["I01"];
        }
    }

    public void PushItemDicLoadRequest()
    {
        foreach(KeyValuePair<string,item> kvp in itemlist)
        {
            kvp.Value.data = LoadManager.instance.getitem(kvp.Value.itemid);
        }
        //for (int i = 0; i < _Itemlist.Count; i++)
        //{
        //    _Itemlist[i].data = LoadManager.instance.getitem(_Itemlist[i].itemid);
        //}
    }

    
    public Dictionary<string, item> get_itemList()
    {
     
            return itemlist;
    }

    public item get_item(string id)
    {
       

        id = id.Substring(0, 1).ToUpper() + id.Substring(1);
        item d = null;
        itemlist.TryGetValue(id, out d);
        return d;
    }
    public void Delete_item(string id)
    {
        itemlist.Remove(id);
    }
    public void Add_item(ItemData d)
    {

        if (itemlist == null)
            itemlist = new Dictionary<string, item>();
        itemlist.Add(d.id,new item() { data = d, itemid = d.id, Amount = 1 });
    }
    public void set_item(item d)
    {
        if (itemlist == null)
            itemlist = new Dictionary<string, item>();
        d.itemid = d.itemid.Substring(0, 1).ToUpper() + d.itemid.Substring(1);
        if (LoadManager.instance.getitem(d.itemid) == null)
        {
            Debug.Log("비정상적인 데이터 입력을 발견, 입력을 종료합니다.");
            return;
        }
    
        if (itemlist.ContainsKey(d.itemid))
        {
            itemlist[d.itemid].Amount += d.Amount;
            if (itemlist[d.itemid].Amount == 0)
                itemlist.Remove(d.itemid);
        }
        else
        {
    
            itemlist.Add(d.itemid, d);
        }
        foreach (KeyValuePair<string, item> kvp in itemlist)
        {
            if (LoadManager.instance.getitem(kvp.Key) == null)
            {
                Debug.Log("비정상적인 데이터를 발견, 데이터를 삭제합니다.");
                itemlist.Remove(kvp.Key);
          
            }
        }
    }
    #endregion
    #region characterlist

    [NonSerialized]
    List<PlayerData> Clist = new List<PlayerData>();

    [SerializeField]
    Dictionary<string, PlayerData> Characterlist=new Dictionary<string, PlayerData>();
    public void DeleteCharacter(string uid)
    {
        Characterlist.Remove(CharacterContentMapping.Canonicalize(uid));
      
    }

    public void AddCharacter(PlayerData d)
    {
        d.id = CharacterContentMapping.Canonicalize(d.id);
        Characterlist.Add(d.id,d);
    }
   
    public Dictionary<string,PlayerData> get_CharacterList()
    {
       
            return Characterlist;
    }
    
    public void loadPlayerSprites()
    {
       
        foreach(KeyValuePair<string,PlayerData> kvp in Characterlist)
        {
            kvp.Value.charactersprite =
                LoadManager.instance.getcharactersprite(
                    kvp.Value.id);
        }
      
    }
   
    public void set_Character(PlayerData d)
    {
        d.id = CharacterContentMapping.Canonicalize(d.id);
     

        if (Characterlist.ContainsKey(d.id))
        {
            Characterlist[d.id] = d;
        }
        else
        {
            Clist.Add(d);
            Characterlist.Add(d.id, d);
            Debug.Log("캐릭터 넣음");
        }
    
           
    
    }
    #endregion
    #region stagelist





    [SerializeField]
    Dictionary<string, StageData> Stagelist = new Dictionary<string, StageData>();
   
    public void DeleteStage(string id)
    {
        Stagelist.Remove(id);
    }
    public Dictionary<string, StageData> get_StageList()
    {
  
            return Stagelist;
    }
    public Dictionary<string, StageData> get_StageList(string s)
    {

      
  
        var kvps=new Dictionary<string, StageData>();

        foreach(KeyValuePair<string,StageData> kvp in Stagelist)
        {
       
            string key= kvp.Key;

            if (key.Contains(s))
            {
        
                kvps.Add(kvp.Key, kvp.Value);
            }
        }


        return kvps;
    }
    public StageData get_Stage(string s)
    {
 
 
        StageData d = null;


        if (Stagelist.TryGetValue(s, out d))
        {

            return d;
        }
        else
        {
            return null;
        }
    }
    public void set_Stage(StageData d)
    {
  //if(Stagelist==null)
  //          Stagelist = new Dictionary<string, StageData>();

        if (Stagelist.ContainsKey(d.ID))
        {
            Debug.Log(d.ID + "변경됨");
            Stagelist[d.ID] = d;
           
        }
        else
        {
            Debug.Log(d.ID+ "추가됨");
            Stagelist.Add(d.ID, d);
           
        }
    }
    #endregion


    [SerializeField]
    public bool TutorialClear1;
    [SerializeField] public bool TutorialClear2;
    [SerializeField]
    int _MaxStamina;
    public int MaxStamina { get { return _MaxStamina; } set { _MaxStamina = value; } }
    [SerializeField]
    int _Stamina;
    public int Stamina { get { return _Stamina; } set { _Stamina = value; } }

    [SerializeField]
    float _StaminaTimer;

    public float StaminaTimer { get { return _StaminaTimer; } set { _StaminaTimer = value; } }
    public PlayerData[] PDatalist { get; set; }

    public void AccountUpdate()
    {
        //LastestConnect = DateTime.Now.ToString("yyyyMMddhhmmss");
        if (Stamina < MaxStamina)
        {
            if (_StaminaTimer > 0)
                _StaminaTimer -= Time.deltaTime;
            else
            {
                _StaminaTimer = global.StaminaCreateTime;

               Stamina++;
             
            }
        }
        //Debug.Log("StageCount" + Stagelist.Count);
    }

    #region network
    internal void NormalizeCharacterIds()
    {
        // Preflight before changing keys or progress objects; never merge aliases.
        var normalized = new Dictionary<string, PlayerData>();
        foreach (var pair in Characterlist)
        {
            string id = pair.Value != null && !string.IsNullOrEmpty(pair.Value.id)
                ? pair.Value.id : pair.Key;
            id = CharacterContentMapping.Canonicalize(id);
            if (normalized.ContainsKey(id))
                throw new InvalidOperationException("Duplicate logical character ID: " + id);
            normalized.Add(id, pair.Value);
        }
        StarChracter = CharacterContentMapping.Canonicalize(StarChracter);
        foreach (var pair in normalized)
            CharacterContentMapping.NormalizeData(pair.Value);
        // Retain the dictionary object exposed by get_CharacterList().
        Characterlist.Clear();
        foreach (var pair in normalized)
            Characterlist.Add(pair.Key, pair.Value);
    }

    public UserAccountDataNetworkMember ReturnNetworkMember()
    {
        NormalizeCharacterIds();
        var a= new UserAccountDataNetworkMember()
        {
            _uid = this._uid,
            username = this.username,
            _accountlv = this.accountlv,
            _Max_Exp = this._Max_Exp,
            _Exp = this._Exp,
            _MaxStamina = this._MaxStamina,
            LastestConnect = this.LastestConnect,
            MaxStageClearData = this.MaxStageClearData,
            StarChracter = CharacterContentMapping.Canonicalize(this.StarChracter),
          Stagelist=new List<StageData>(),
          Characterlist=new List<PlayerData>(),
          Itemlist=new List<item>(),
            TutorialClear1 = this.TutorialClear1,
            TutorialClear2 = this.TutorialClear2,
            _Stamina = this._Stamina,
            _StaminaTimer = this._StaminaTimer

        };
        foreach(KeyValuePair<string, PlayerData> kvp in Characterlist)
        {
            a.Characterlist.Add(CharacterContentMapping.NormalizeData(kvp.Value));
        }
        foreach (KeyValuePair<string, StageData> kvp in Stagelist)
        {
            a.Stagelist.Add(kvp.Value);
        }
        foreach (KeyValuePair<string, item> kvp in itemlist)
        {
            a.Itemlist.Add(kvp.Value);
        }
        return a;
     
    }
    public void GetNetworkMember(UserAccountDataNetworkMember member)
    {
        CharacterContentMapping.NormalizeData(member);
        _uid = member._uid;
        username = member.username;
        _accountlv = member._accountlv;
        _Max_Exp = member._Max_Exp;
        _Exp = member._Exp;
        _MaxStamina = member._MaxStamina;
        LastestConnect = member.LastestConnect;
        MaxStageClearData = member.MaxStageClearData;
        StarChracter = member.StarChracter;
    
        TutorialClear1 = member.TutorialClear1;
        TutorialClear2 = member.TutorialClear2;
        _Stamina = member._Stamina;
        _StaminaTimer = member._StaminaTimer;
        //Stagelist.Clear();
        itemlist.Clear();
        Characterlist.Clear();

        foreach (PlayerData kvp in member.Characterlist)
        {
            Characterlist.Add(kvp.id,kvp);
        }
        foreach (StageData kvp in member.Stagelist)
        {
            if(Stagelist.ContainsKey(kvp.ID))
            {
                Stagelist[kvp.ID] = kvp;
            }
            else
                Stagelist.Add(kvp.ID, kvp);
        }
        foreach (item kvp in member.Itemlist)
        {
            itemlist.Add(kvp.itemid,kvp);
        }
    }
    #endregion

}

public class global
{
    public static int StaminaCreateTime = 300;
   
}
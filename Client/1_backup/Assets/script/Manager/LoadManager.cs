using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Reflection;
using menu;
using Newtonsoft.Json;

public class LoadManager : MonoBehaviour
{
   //캐릭터 스프라이트도 아이템이랑 비슷하게 미리 로드하는 식으로


    public bool onload;
    Queue<IEnumerator> LoadingQueue;
    Dictionary<string, Sprite> CharacterSpriteDictionary;
    public Sprite getcharactersprite(string s)
    {
        Debug.Log(s);
        s = s.Substring(0, 1).ToUpper() + s.Substring(1);

        Sprite spr;
        if (CharacterSpriteDictionary.TryGetValue(s, out spr))
        {
            return spr;
        }
        else
        {
            Debug.Log("Can't Find Item");
            return null;
        }
    }
    Dictionary<string, ItemData> itemDictionary;
    public ItemData getitem(string s)
    {

        s = s.Substring(0, 1).ToUpper() + s.Substring(1);
      
        ItemData i;
        if (itemDictionary.TryGetValue(s, out i))
        {
            return i;
        }
        else
        {
            Debug.Log("Can't Find Item");
            return null;
        }
    }

    public void DataLoadExecute()
    {
        StartCoroutine(LoadingStart());
    }
    IEnumerator LoadingStart()
    {
        Loadingprogress = 0;

            GameManager.instance.ActiveLoadingImg();
        yield return StartCoroutine(processQueueAsync());
        GameManager.instance.DeactiveLoadingImg();
        MaxLoadingprogress = 0;
        Loadingprogress = 0;
    }
    IEnumerator processQueueAsync()
    {
        onload = true;
        GameManager.instance.ActiveLoadingImg();
     


        while (LoadingQueue.Count != 0)
        {
            IEnumerator corutine = LoadingQueue.Dequeue();
            yield return StartCoroutine(corutine);

        }
     



        onload = false;
        GameManager.instance.DeactiveLoadingImg();

    }
    #region load method
    public string ReturnChrDataName(CharacterData data)
    {
      
            AssetBundle bundle = AssetBundle.LoadFromFile(bundleDirectory + "/characterdb");
            var a = bundle.LoadAsset(data.id);
            CharacterInformation b = convertTextasset<CharacterInformation>(a as TextAsset);
        bundle.Unload(false);
            return b.name;
       
    }
    public void TryLoadItem(item i)
    {
        if (itemDictionary == null)
            itemDictionary = new Dictionary<string, ItemData>();
        ItemData data=null;
        if(itemDictionary.TryGetValue(i.itemid,out data))
        {
            i.data = data;
        }
        else
        {
            PushitemLoadRequest(i.itemid, (data) =>
            {
                itemDictionary.Add(i.itemid, data);
                i.data = data;
            });
        }
    }
    public void TryLoadItem(List< item> i)
    {
        if (itemDictionary == null)
            itemDictionary = new Dictionary<string, ItemData>();
        ItemData data = null;
        foreach (item obj in i)
        {
            if (itemDictionary.TryGetValue(obj.itemid, out data))
            {
                obj.data = data;
            }
            else
            {
                PushitemLoadRequest(obj.itemid, (data) =>
                {
                    itemDictionary.Add(obj.itemid, data);
                    obj.data = data;
                });
            }
        }
    }
    public void PushCharacterSFX(string sfxname,Action<AudioClip> action)
    {
        LoadingQueue.Enqueue(LoadCharacterSfxasync("charactersfx",sfxname,action));
        MaxLoadingprogress += 2;
    }
    public void PushCharacterloadrequest(string s, Action<GameObject> action)
    {
        LoadingQueue.Enqueue(LoadPrefabAsync("character", s, action));
        MaxLoadingprogress += 2;
    }
    public void PushCharacterloadrequest(string bundlename,string s, Action<GameObject> action)
    {
        LoadingQueue.Enqueue(LoadPrefabAsync(bundlename, s, action));
        MaxLoadingprogress += 2;
    }
    public void PushCharactersloadrequest(List<String> s, Action<List<GameObject>> action)
    {
        LoadingQueue.Enqueue(LoadPrefabsAsync("character", s, action));
        MaxLoadingprogress += s.Count+1;
    }
    public void PushCharacterSpriteloadrequest(string s, Action<Sprite> action)
    {
      
        LoadingQueue.Enqueue(LoadSpriteAsync("charactersprite", s, action));
        MaxLoadingprogress += 2;
    }
    public void PushCharacterSpriteloadrequest(string bundlename,string s, Action<Sprite> action)
    {

        LoadingQueue.Enqueue(LoadSpriteAsync(bundlename, s, action));
        MaxLoadingprogress += 2;
    }
    public void PushCharactersSpriteloadrequest(List<String> s, Action<List<Sprite>> action)
    {
        LoadingQueue.Enqueue(LoadSpritesAsync("charactersprite", s, action));
        MaxLoadingprogress += s.Count + 1;
    }
   public void PushItemspriteLoadRequest(string s,Action<Sprite> action)
    {
        LoadingQueue.Enqueue(LoadSpriteAsync("itemsprite", s, action));
        MaxLoadingprogress += 2;
    }
    public void PushLoadTerrainRequest(string s,Action<GameObject> action)
    {
        LoadingQueue.Enqueue(LoadPrefabAsync("StageTerrain", s, action));
        MaxLoadingprogress += 2;
    }
    public void PushItemspriteLoadRequest(string bundlename,string s, Action<Sprite> action)
    {
        LoadingQueue.Enqueue(LoadSpriteAsync(bundlename, s, action));
        MaxLoadingprogress += 2;
    }
    public void PushItemsSpriteLoadRequest(List<String> s, Action<List<Sprite>> action)
    {
        LoadingQueue.Enqueue(LoadSpritesAsync("itemsprite", s, action));
        MaxLoadingprogress += s.Count + 1;
    }
    public void PushCharacteranimatorloadrequest(string s, Action<RuntimeAnimatorController> action)
    {
       
        LoadingQueue.Enqueue(LoadAnimatorAsync("characteranimation", s, action));
        MaxLoadingprogress += 2;
    }
    public void PushCharacteranimatorsloadrequest(List<String> s, Action<List<RuntimeAnimatorController>> action)
    {
        LoadingQueue.Enqueue(LoadAnimatorsAsync("characteranimation", s, action));
        MaxLoadingprogress += s.Count + 1;
    }

    public void PushCharacterdbLoadRequest(string ID, Action<CharacterInformation> action)
    {
        LoadingQueue.Enqueue(LoadTextAssetAsync("characterdb", ID, action));
        MaxLoadingprogress += 2;
    }
    public void PushCharacterdbsLoadRequest(List<String> ID, Action<List<CharacterInformation>> action)
    {
        LoadingQueue.Enqueue(LoadTextAssetsAsync("characterdb", ID, action));
        MaxLoadingprogress += ID.Count+1;
    }
    public void PushStagedbLoadRequest(string ID, Action<StageInformation> action)
    {
        LoadingQueue.Enqueue(LoadTextAssetAsync("stagedb", ID, action));
        MaxLoadingprogress += 2;
    }
    public void PushStageDownLoadRequest()
    {
        LoadingQueue.Enqueue(LoadAllTextAssetsAsync<StageInformation>("stagedb", (list)=>
        {
           
            List<string> StageinfoIDs=new List<string>();
           
            //데이터 추가
            foreach (StageInformation item in list) {
                if(
                !GameManager.instance.Account.get_StageList().ContainsKey(item.ID))
                {

                    GameManager.instance.Account.set_Stage(new StageData()
                    {
                        ID = item.ID
                        ,
                        name = item.name,
                        stagemission = item.missionarray,
                        stagelock = true
                    });
                  
                }
                }
          
        }));
    }
    public void PushitemdbLoadRequest(string ID, Action<ItemData> action)
    {
        LoadingQueue.Enqueue(LoadTextAssetAsync("itemdb", ID, action));
        MaxLoadingprogress += 2;
    }
    public void PushitemdbsLoadRequest(List<string> ID, Action<List< ItemData>> action)
    {
        LoadingQueue.Enqueue(LoadTextAssetsAsync("itemdb", ID, action));
        MaxLoadingprogress += 2;
    }
    public void PushitemLoadRequest(string ID, Action<ItemData> action)
    {
        LoadingQueue.Enqueue(LoaditemAsync( ID, action));
        MaxLoadingprogress += 2;
    }
    public void PushitemsLoadRequest(List<string> ID, Action<List<ItemData>> action)
    {
        LoadingQueue.Enqueue(LoaditemsAsync( ID, action));
        MaxLoadingprogress += 2;
    }
    #endregion

    #region LoadCorutine
  public IEnumerator LoadCharacterSfxasync(string bundlename,string assetname,Action<AudioClip> act)
    {
        if (!File.Exists(bundleDirectory + $"\\{bundlename}"))
        {
            AudioClip clip = null;
            act.Invoke(clip);
            yield break;
        }
        AssetBundleCreateRequest AssetBundleCreate =AssetBundle.LoadFromFileAsync(
 bundleDirectory + $"\\{bundlename}");
        while (!AssetBundleCreate.isDone)
        {
            yield return null;
        }
        AssetBundle bundle = AssetBundleCreate.assetBundle;
        var request = bundle.LoadAssetAsync(assetname);
        while (!request.isDone)
        {
            yield return null;

        }
        var a=request.asset as AudioClip;
        act.Invoke(a);
        bundle.Unload(false);
    }

    public IEnumerator LoadCharacterSfxsasync(string bundlename, List<string> assetnames, Action<List<AudioClip>> act)
    {
       
        AssetBundleCreateRequest AssetBundleCreate = AssetBundle.LoadFromFileAsync(
 bundleDirectory + $"\\{bundlename}");
        while (!AssetBundleCreate.isDone)
        {
            yield return null;
        }
        AssetBundle bundle = AssetBundleCreate.assetBundle;
        List<AudioClip> audioClips = new List<AudioClip>();
        foreach (string assetname in assetnames)
        {
            var request = bundle.LoadAssetAsync(assetname);
            while (!request.isDone)
            {
                yield return null;

            }
            var a = request.asset as AudioClip;
            audioClips.Add(a);
        }

        act.Invoke(audioClips);
    }
    IEnumerator LoadAssetBundleAsync(string bundlename)
    {
        if (!File.Exists(bundleDirectory + $"\\{bundlename}"))
        {
            Debug.Log("Can't find bundle");
            yield break;
        }
        AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
bundleDirectory + $"\\{bundlename}");
        while (!assetBundleCreate.isDone)
        {
            yield return null;
        }
        AssetBundle bundle = assetBundleCreate.assetBundle;
 
       
    }
    public  IEnumerator CheckBundleVersion(string bundlename,Action<string> act)
    {
        //GameManager.instance.ActiveChasingLog(bundleDirectory + $"\\{bundlename}");
        if (!File.Exists(bundleDirectory + $"\\{bundlename}"))
        {
            string s = "none";
            act.Invoke(s);
            yield break;
        }
            AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
 bundleDirectory + $"\\{bundlename}");
            while (!assetBundleCreate.isDone)
            {
                yield return null;
            }
            AssetBundle bundle = assetBundleCreate.assetBundle;
        if (bundle == null)
        {
            string s = "none";
            act.Invoke(s);
            yield break;
        }
        var request = bundle.LoadAssetAsync("BundleVersion");
        if (request == null)
        {
            string s = "none";
            act.Invoke(s);
            yield break;
        }
        while (!request.isDone)
        {
            yield return null;

        }

        var Ver = request.asset as TextAsset;
        var str = Ver.text.Substring(8);
        str = str.Replace(" ", "");
 
        bundle.Unload(false);
        act.Invoke(str);
        
    }
    IEnumerator LoadPrefabAsync(string bundlename,string assetname, Action<GameObject> act)
    {
        Debug.Log(bundlename + "," + assetname);
        AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
  bundleDirectory + $"\\{bundlename}");
        while (!assetBundleCreate.isDone)
        {
            yield return null;
            Loadingprogress = processingcount + assetBundleCreate.progress;

        }
        processingcount++;


        //yield return assetBundleCreate;
        

        AssetBundle bundle = assetBundleCreate.assetBundle;
        if (bundle == null)
        {
            Debug.Log("bundle find error");
            yield break;
        }
   
        var request = bundle.LoadAssetAsync(assetname);

        while (!request.isDone)
        {
            yield return null;
            Loadingprogress = processingcount + request.progress;
        }
        processingcount++;
        //yield return new WaitUntil(() => request.isDone);



        if (request == null)
        {
            Debug.Log("asset find error");
            yield break;
        }


        act.Invoke(request.asset as GameObject);
        bundle.Unload(false);

    }
    IEnumerator LoadPrefabsAsync(string bundlename, List<string> assetnames, Action<List<GameObject>> act)
    {
        
        AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
  bundleDirectory + $"\\{bundlename}");
        while (!assetBundleCreate.isDone)
        {
            yield return null;
            Loadingprogress = processingcount + assetBundleCreate.progress;

        }
        processingcount++;
        //yield return assetBundleCreate;
        var bundle = assetBundleCreate.assetBundle;
        if (bundle == null)
        {
            Debug.Log("bundle find error");
            yield break;
        }
        List<GameObject> obj = new List<GameObject>();
        foreach (string assetname in assetnames)
        {
            var request = bundle.LoadAssetAsync(assetname);

            while (!request.isDone)
            {
                yield return null;
                Loadingprogress = processingcount + request.progress;
            }
            processingcount++;

            //yield return request;
            if (request == null)
            {
                Debug.Log("asset find error");
                yield break;
            }
            obj.Add(request.asset as GameObject);
        }
        act.Invoke(obj);
        bundle.Unload(false);
    }




  
   
    IEnumerator LoadSpriteAsync(string bundlename,string assetname, Action<Sprite> act)
    {

        AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
  bundleDirectory + $"\\{bundlename}");
        while (!assetBundleCreate.isDone)
        {
            
            yield return null;
            Loadingprogress = processingcount + assetBundleCreate.progress;

        }
        processingcount++;


        //yield return assetBundleCreate;


        AssetBundle bundle = assetBundleCreate.assetBundle;
        if (bundle == null)
        {
            Debug.Log("bundle find error");
            yield break;
        }

        var request = bundle.LoadAssetAsync(assetname);

        while (!request.isDone)
        {
            yield return null;
            Loadingprogress = processingcount + request.progress;
        }
        processingcount++;
        //yield return new WaitUntil(() => request.isDone);



        if (request == null)
        {
            Debug.Log("asset find error");
            yield break;
        }

        Texture2D texture = request.asset as Texture2D;

        if (texture != null)
        {
           
            act.Invoke(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)));
        }
            bundle.Unload(false);

    }
    IEnumerator LoadSpritesAsync(string bundlename, List<string> assetnames, Action<List<Sprite>> act)
    {

        AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
  bundleDirectory + $"\\{bundlename}");
        while (!assetBundleCreate.isDone)
        {
            yield return null;
            Loadingprogress = processingcount + assetBundleCreate.progress;

        }
        processingcount++;
        //yield return assetBundleCreate;
        var bundle = assetBundleCreate.assetBundle;
        if (bundle == null)
        {
            Debug.Log("bundle find error");
            yield break;
        }
        List<Sprite> obj = new List<Sprite>();
        foreach (string assetname in assetnames)
        {
            var request = bundle.LoadAssetAsync(assetname);

            while (!request.isDone)
            {
                yield return null;
                Loadingprogress = processingcount + request.progress;
            }
            processingcount++;

            //yield return request;
            if (request == null)
            {
                Debug.Log("asset find error");
                yield break;
            }
            Texture2D texture = request.asset as Texture2D;
            if(texture!=null)
                obj.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)));
        }
        act.Invoke(obj);
        bundle.Unload(false);
    }
    IEnumerator LoadAllCharacterSpritesAsync()
    {
        CharacterSpriteDictionary = new Dictionary<string, Sprite>();
        AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
  bundleDirectory + "\\charactersprite");
        while (!assetBundleCreate.isDone)
        {
            yield return null;
            Loadingprogress = processingcount + assetBundleCreate.progress;

        }
        processingcount++;
        //yield return assetBundleCreate;
        var bundle = assetBundleCreate.assetBundle;
        if (bundle == null)
        {
            Debug.Log("bundle find error");
            yield break;
        }
        var requests = bundle.LoadAllAssetsAsync<Texture2D>();

        yield return requests;
        if (requests == null)
        {
            Debug.Log("asset find error");
            yield break;
        }
        UnityEngine.Object[] datas = requests.allAssets;
        MaxLoadingprogress += datas.Length;
       
        foreach (Texture2D data in datas)
        {



            Sprite s =
            Sprite.Create(data, new Rect(0, 0, data.width, data.height), new Vector2(0.5f, 0.5f));

           
            string id=
            data.name.Substring(0, 1).ToUpper() + data.name.Substring(1);
            CharacterSpriteDictionary.Add(id, s);

            Loadingprogress++;
        }
        bundle.Unload(false);
    }
    IEnumerator LoadAnimatorAsync(string bundlename,string assetname, Action<RuntimeAnimatorController> act)
    {

        AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
  bundleDirectory + $"\\{bundlename}");
        while (!assetBundleCreate.isDone)
        {
            yield return null;
            Loadingprogress = processingcount + assetBundleCreate.progress;

        }
        processingcount++;



        AssetBundle bundle = assetBundleCreate.assetBundle;
        if (bundle == null)
        {
            Debug.Log("bundle find error");
            yield break;
        }

        var request = bundle.LoadAssetAsync(assetname);

        while (!request.isDone)
        {
            yield return null;
            Loadingprogress = processingcount + request.progress;
        }
        processingcount++;



        if (request == null)
        {
            Debug.Log("asset find error");
            yield break;
        }


        act.Invoke(request.asset as RuntimeAnimatorController);
        bundle.Unload(false);

    }
    IEnumerator LoadAnimatorsAsync(string bundlename, List<string> assetnames, Action<List<RuntimeAnimatorController>> act)
    {

        AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
  bundleDirectory + $"\\{bundlename}");
        while (!assetBundleCreate.isDone)
        {
            yield return null;
            Loadingprogress = processingcount + assetBundleCreate.progress;

        }
        processingcount++;
        //yield return assetBundleCreate;
        var bundle = assetBundleCreate.assetBundle;
        if (bundle == null)
        {
            Debug.Log("bundle find error");
            yield break;
        }
        List<RuntimeAnimatorController> obj = new List<RuntimeAnimatorController>();
        foreach (string assetname in assetnames)
        {
            var request = bundle.LoadAssetAsync(assetname);
   
            while (!request.isDone)
            {
                yield return null;
                Loadingprogress = processingcount + request.progress;
            }
            processingcount++;

            //yield return request;
            if (request == null)
            {
                Debug.Log("asset find error");
                yield break;
            }
            obj.Add(request.asset as RuntimeAnimatorController);
        }
        act.Invoke(obj);
        bundle.Unload(false);
    }
    IEnumerator LoadTextAssetAsync<T>(string bundlename,string assetname, Action<T> act)
    {


        AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
  bundleDirectory + $"\\{bundlename}");
      
        yield return assetBundleCreate;
  
        var bundle = assetBundleCreate.assetBundle;
        if (bundle == null)
        {
            Debug.Log("bundle find error");
            yield break;
        }

        var request = bundle.LoadAssetAsync(assetname);

        yield return request;

        if (request == null)
        {
            Debug.Log("asset find error");
            yield break;
        }
        act.Invoke(convertTextasset<T>(request.asset as TextAsset));
        bundle.Unload(false);
    }
    IEnumerator LoadTextAssetsAsync<T>(string bundlename, List<string> assetnames, Action<List<T>> act)
    {

        AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
  bundleDirectory + $"\\{bundlename}");

        yield return assetBundleCreate;
        var bundle = assetBundleCreate.assetBundle;
        if (bundle == null)
        {
            Debug.Log("bundle find error");
            yield break;
        }
        List<T> obj = new List<T>();
        foreach (string assetname in assetnames)
        {
            var request = bundle.LoadAssetAsync(assetname);

            yield return request;
            if (request == null)
            {
                Debug.Log("asset find error");
                yield break;
            }
            obj.Add(convertTextasset<T>( request.asset as TextAsset));
        }
        act.Invoke(obj);
        bundle.Unload(false);
    }
    IEnumerator LoadAllTextAssetsAsync<T>(string bundlename, Action<List<T>> act)
    {

        AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
  bundleDirectory + $"\\{bundlename}");

        yield return assetBundleCreate;
        var bundle = assetBundleCreate.assetBundle;
        if (bundle == null)
        {
            Debug.Log("bundle find error");
            yield break;
        }
        List<T> obj = new List<T>();
        var request = bundle.LoadAllAssets();
        foreach (TextAsset item in request)
        {
            if (item.text[..7] == "Version")
                continue;
            else
            {
                obj.Add(convertTextasset<T>(item as TextAsset));
            }
        }
        act.Invoke(obj);
        bundle.Unload(false);
    }
    IEnumerator LoaditemAsync( string assetname, Action<ItemData> act)
    {
        ItemData data=null;
        yield return StartCoroutine(LoadTextAssetAsync<ItemData>("itemdb", assetname, (d) => data = d));
        yield return StartCoroutine(LoadSpriteAsync("itemsprite", assetname, (s) => { if (data != null) data.itemSprite = s; }));
        act.Invoke(data);

    }
    IEnumerator LoaditemsAsync(List<string> assetnames, Action<List<ItemData>> act)
    {
        List<ItemData> datas = null;
        yield return StartCoroutine(LoadTextAssetsAsync<ItemData>("itemdb", assetnames, (d) => datas = d));
        yield return StartCoroutine(LoadSpritesAsync("itemsprite", assetnames, (s) => { for (int n = 0; n < s.Count; n++) { datas[n].itemSprite = s[n]; } }));
        act.Invoke(datas);

    }

    IEnumerator LoaditemDictionaryAsync(string assetname, Action<ItemData> act)
    {
        ItemData data = null;
        yield return StartCoroutine(LoadTextAssetAsync<ItemData>("itemdb", assetname, (d) => data = d));
        yield return StartCoroutine(LoadSpriteAsync("itemsprite", assetname, (s) => { if (data != null) data.itemSprite = s; }));
        act.Invoke(data);
    }
    IEnumerator LoadAllitemAsync()
    {
     
        itemDictionary = new Dictionary<string, ItemData>();
        AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
 bundleDirectory + "\\itemdb");

        yield return assetBundleCreate;


        var bundle = assetBundleCreate.assetBundle;
        if (bundle == null)
        {
            Debug.Log("bundle find error");
            yield break;
        }
        var requests = bundle.LoadAllAssetsAsync<TextAsset>();

        yield return requests;

        if (requests == null)
        {
            Debug.Log("asset find error");
            yield break;
        }

        UnityEngine.Object[] datas = requests.allAssets;
        MaxLoadingprogress += datas.Length;
        foreach (TextAsset data in datas)
        {
          string text=data.text;
            if (text[..7] == "Version")
            {

                Loadingprogress++;
                continue;
            }
            else
            {
                ItemData item = convertTextasset<ItemData>(data);
                yield return LoadSpriteAsync("itemsprite", item.id, (spr) => { item.itemSprite = spr; });
                itemDictionary.Add(item.id, item);
                Loadingprogress++;
            }
        }
        bundle.Unload(false);
     
    }

    #endregion




    public static LoadManager instance;
    string bundleDirectory;
    private void Awake()
    {
        LoadingQueue = new Queue<IEnumerator>();
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        //bundleDirectory = Directory.GetCurrentDirectory() + "\\Assets\\StreamingAssets\\Assetbundle";
        bundleDirectory = Application.streamingAssetsPath+"/Assetbundle";
    }

    T convertTextasset<T>(TextAsset Text)
    {
       return JsonUtility.FromJson<T>(Text.text);
    }
  
    [HideInInspector]
   public float Loadingprogress;
    [HideInInspector]
    public float MaxLoadingprogress;
    float processingcount;
    public TextAsset DummyAccount;
    public UserAccountData loaddummyAccount()
    {
        Debug.Log("더미 계정 생성");
        var networkMember = convertTextasset<UserAccountDataNetworkMember>(DummyAccount);

        // 2. 실제 데이터 클래스 생성 및 리스트 초기화
        UserAccountData dummy = new UserAccountData();
        dummy.initializelist();

        // 3. GetNetworkMember 함수를 통해 List 데이터를 Dictionary로 안전하게 변환
        dummy.GetNetworkMember(networkMember);

        return dummy;
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene("Loading");
       
        StartCoroutine(loadMenuScene()    );

    }
    IEnumerator loadMenuScene()
    {
        if (GameManager.instance.Account.StarChracter == "")
        {
            GameManager.instance.Account.StarChracter = "P01";
        }
        PushCharacterloadrequest(GameManager.instance.Account.StarChracter, (o) => { GameManager.instance.Account.StarCharacterModel = o; });
       
        yield return StartCoroutine(LoadAllitemAsync());
        Debug.Log("아이템 호출");
        yield return StartCoroutine (LoadAllCharacterSpritesAsync());
        Debug.Log("캐릭터 호출");
        GameManager.instance.Account.loadPlayerSprites();

        GameManager.instance.Account.PushItemDicLoadRequest();
        yield return StartCoroutine(loadingsceneAsync("MainMenu"));
        yield return new WaitUntil(() =>{ if (menu.Menu.instance!=null) return true; else return false; }) ;

        menu.Menu.instance.ActiveMainMenu();
        
    }
 
    public IEnumerator Checkbunddle()
    {
       
        //#1
        yield return null;
        //#2

        //#3


      
    }
  
    IEnumerator loadingsceneAsync(string s)
    {
        MaxLoadingprogress += 1;
        Loadingprogress = 0;
        yield return StartCoroutine(processQueueAsync());
       AsyncOperation async = SceneManager.LoadSceneAsync(s);
        async.allowSceneActivation = false;
   


        //float loadingMinTImer = 0;
        while (true)
        {
            yield return null;



            Loadingprogress =processingcount+ async.progress;

                if (async.progress>=0.65f)
                {

                   
                async.allowSceneActivation = true;
                processingcount = 0;
                Loadingprogress = 0;
                MaxLoadingprogress = 0;
                yield break;
                }

         
        }
    }

    string selectstage;
   public void loadgame(StageData stagedata,StageInformation info, PlayerData[] players)
    {
        SceneManager.LoadScene("Loading");
        selectstage = stagedata.ID;
        
        StartCoroutine(loadingGamesceneAsync(stagedata,info, players));
    }

 
    IEnumerator loadingGamesceneAsync_T(StageData stagedata,StageInformation info, PlayerData[] players)
    {
       

        List<string> CharacterID = new List<string>();


        int playernumber = 0;
        foreach (PlayerData data in players)
        {
            if (data == null)
                break;
            else
            {
                CharacterID.Add(data.id);
                playernumber++;
            }
        }

        foreach (CharacterData data in info.SpawnCharacterList)
        {
            CharacterID.Add(data.id);
        }
        List<GameObject> loadedcharacter = new List<GameObject>();
        PushCharactersloadrequest(CharacterID, (list) =>
        {

            loadedcharacter = list;
        });
        PushCharacterdbsLoadRequest(CharacterID,
                (list) =>
                {
                    for (int i = 0; i < playernumber; i++)
                    {
                        loadedcharacter[i].GetComponent<GameCharacter>().set_Character(list[i], players[i]);
                    }
                    for (int i = playernumber; i < list.Count; i++)
                    {
                        loadedcharacter[i].GetComponent<GameCharacter>().set_Character(list[i], info.SpawnCharacterList[i - playernumber]);
                    }
                });
        PushCharacteranimatorsloadrequest(CharacterID, (List) =>
        {
            for (int i = 0; i < playernumber; i++)
            {
                loadedcharacter[i].GetComponent<GameCharacter>().setAnimation(List[i]);
            }
            for (int i = playernumber; i < List.Count; i++)
            {
                loadedcharacter[i].GetComponent<GameCharacter>().setAnimation(List[i]);
            }
        });
        //이게 이렇게 힘들게 로드해야 하는 일인가....
        PushCharacterSFX("Hit", (Sounds) =>
        {
            Character_Audio_Player AudioPlayer;
            foreach(GameObject obj in loadedcharacter)
            {
                if(obj.TryGetComponent<Character_Audio_Player>(out AudioPlayer))
                {
                    AudioPlayer.HitClip= Sounds;
                }
            }
        });
      
        yield return StartCoroutine(processQueueAsync());

        yield return StartCoroutine(loadingsceneAsync(StageSceneName));

        ; yield return new WaitUntil(() => Stage._stage != null);
        Stage._stage.setStage(info, loadedcharacter, players, playernumber);


    }

    IEnumerator loadingGamesceneAsync(StageData stagedata, StageInformation info, PlayerData[] players)
    {


        List<string> CharacterID = new List<string>();


        int playernumber = 0;
        foreach (PlayerData data in players)
        {
            if (data == null)
                break;
            else
            {
                CharacterID.Add(data.id);
                playernumber++;
            }
        }

        foreach (CharacterData data in info.SpawnCharacterList)
        {
            CharacterID.Add(data.id);
        }
        List<GameObject> loadedcharacter = new List<GameObject>();

        GameObject Terrain=null;
        PushLoadTerrainRequest(info.TerrainDataName, (data) => Terrain = data);
        PushCharactersloadrequest(CharacterID, (list) =>
        {

            loadedcharacter = list;
        });
        PushCharacterdbsLoadRequest(CharacterID,
                (list) =>
                {
                    for (int i = 0; i < playernumber; i++)
                    {
                        loadedcharacter[i].GetComponent<GameCharacter>().set_Character(list[i], players[i]);
                    }
                    for (int i = playernumber; i < list.Count; i++)
                    {
                        loadedcharacter[i].GetComponent<GameCharacter>().set_Character(list[i], info.SpawnCharacterList[i - playernumber]);
                    }
                });
        PushCharacteranimatorsloadrequest(CharacterID, (List) =>
        {
            for (int i = 0; i < playernumber; i++)
            {
                loadedcharacter[i].GetComponent<GameCharacter>().setAnimation(List[i]);
            }
            for (int i = playernumber; i < List.Count; i++)
            {
                loadedcharacter[i].GetComponent<GameCharacter>().setAnimation(List[i]);
            }
        });
        PushCharacterSFX("Hit", (Sounds) =>
        {
            Character_Audio_Player AudioPlayer;
            foreach (GameObject obj in loadedcharacter)
            {
                if (obj.TryGetComponent<Character_Audio_Player>(out AudioPlayer))
                {
                    AudioPlayer.HitClip = Sounds;
                }
            }
        });

        yield return StartCoroutine(processQueueAsync());

        yield return StartCoroutine(loadingsceneAsync(StageSceneName));

        ; yield return new WaitUntil(() => Stage._stage != null);
        Stage._stage.setStage(info, loadedcharacter, players, playernumber,Terrain);


    }



    public void LoadTitle()
    {
        SceneManager.LoadScene("Loading");
        StartCoroutine(loadTtitleAsync());
    }
    IEnumerator loadTtitleAsync()
    {
        GameManager.instance.Account=null;
        GameManager.instance.DisConnectServer();
        yield return StartCoroutine(loadingsceneAsync("Title"));
    }
    public void ReLoadGame(StageInformation info, List<GameObject> characters, List<GameObject>Players, PlayerData[] playerdatas)
    {
        SceneManager.LoadScene("Loading");
        StartCoroutine(ReloadingGamesceneAsync(info,characters, Players, playerdatas));
    }
    string StageSceneName = "StageTest";
    //string StageSceneName = "Stage";
    IEnumerator ReloadingGamesceneAsync(StageInformation info,List<GameObject> characters, List<GameObject> Players, PlayerData[] playerdatas)
    {
        yield return StartCoroutine(loadingsceneAsync(StageSceneName));

        yield return new WaitUntil(() => Stage._stage != null);


        Stage._stage.ResetStage(info,characters, Players, playerdatas);
        Debug.Log("리로딩 완료");
    }

    public void BackMainMenu(StageInformation info)
    {
        SceneManager.LoadScene("Loading");
      
        StartCoroutine(BackMainMenusceneAsync( info));
    }
    
     IEnumerator BackMainMenusceneAsync(StageInformation info)
    {
        yield return StartCoroutine(loadingsceneAsync("MainMenu"));
        yield return Menu.instance;
        if (selectstage.Contains("Main"))
        {
            Menu.instance.ActiveUI(Menu.instance.MainUI.gameObject);
            Menu.instance.StageUI.transform.parent.gameObject.SetActive(true);
            Menu.instance.ActiveUI(Menu.instance.StageUI.gameObject);
            Menu.instance.ActiveUI(Menu.instance.StageUI.MainStageSelectedUI);
            int n = int.Parse(selectstage.Substring(4, 1));
            Menu.instance.StageUI.MainStageSelectedUI.GetComponent<MainStageSelectedUI>().getStageinfo("Main", n);
        }



        if (Menu.instance.LastestUI.Count == 0)
        {
            Menu.instance.LastestUI.Push(Menu.instance.MainUI.gameObject);
           Menu.instance.LastestUI.Push(Menu.instance.StageUI.gameObject);
        }
       
        selectstage = null;
    }
}

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

public partial class LoadManager
{
    string bundleDirectory;

    private string ReturnChrDataNameAssetBundleLegacy(CharacterData data)
    {
      
            string assetName = CharacterContentMapping.Get(data.id).JsonName;
            AssetBundle bundle = AssetBundle.LoadFromFile(bundleDirectory + "/characterdb");
            var a = bundle.LoadAsset(assetName);
            CharacterInformation b = convertTextasset<CharacterInformation>(a as TextAsset);
        bundle.Unload(false);
            return b.name;
       
    }
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
        CharacterSpriteDictionary = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
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
            data.name;
            CharacterSpriteDictionary.Add(id, s);

            Loadingprogress++;
        }
        bundle.Unload(false);
    }
    IEnumerator LoadAnimatorAsync(string bundlename,string assetname, Action<RuntimeAnimatorController> act)
    {
        if (assetname == null)
        {
            processingcount += 2;
            Loadingprogress = processingcount;
            act.Invoke(null);
            yield break;
        }

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
            if (assetname == null)
            {
                obj.Add(null);
                processingcount++;
                Loadingprogress = processingcount;
                continue;
            }
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
}

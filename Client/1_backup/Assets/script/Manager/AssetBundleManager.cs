using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Assetload
{
    public class AssetBundleManager:MonoBehaviour
    {
        string bundleDirectory;
        AssetBundleManager(){
            bundleDirectory= Directory.GetCurrentDirectory() + "\\Assets\\StreamingAssets\\Assets";
        }

        public void Loadasset(string bundlename,string assetname)
        {
           
        }
        public AssetBundle Character;
        public AssetBundle CharacterDB;
        public bool AssetBundleLoadComplete;
        public void Loadbasicasset()
        {
            
        }

        IEnumerator LoadbasicAssetbundleasync()
        {
            AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromFileAsync(
           bundleDirectory + "\\Character");
            yield return assetBundleCreate;
            Character = assetBundleCreate.assetBundle;
            if (Character == null)
            {
                Debug.LogError("Character loading Failed");
                yield break;
            }
            Character.Unload(false);
            assetBundleCreate = AssetBundle.LoadFromFileAsync(
          bundleDirectory + "\\CharacterDB");
            yield return assetBundleCreate;
            CharacterDB = assetBundleCreate.assetBundle;
            if (Character == null)
            {
                Debug.LogError("DataBase loading Failed");
                yield break;
            }
            CharacterDB.Unload(false);
            AssetBundleLoadComplete = true;
        }
        object obj;
        bool loadingcomplete;
        float MaxLoadingWaitingTime;
        public void LoadCharacterDB(string dbname)
        {

            

        }
        IEnumerator LoadCharacterDBAssetasync(string assetname)
        {
           
           
            AssetBundleRequest assetRequest = CharacterDB.LoadAssetAsync<GameObject>(assetname);
            yield return null;
            obj = assetRequest;


        }

    }
}
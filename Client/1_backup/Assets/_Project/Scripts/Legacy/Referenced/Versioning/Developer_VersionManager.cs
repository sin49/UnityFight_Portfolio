using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class Developer_VersionManager : MonoBehaviour
{
   public TextMeshProUGUI ApplicationVersion;
    public List<Developer_VersionCheck> VersionCheckers;
    public List<string> FilePaths;
    public List<string> FileName;
    public Button WriteButton;
    string assetBundleDir = Directory.GetCurrentDirectory() + "\\Assets\\StreamingAssets" + "\\Assets";
    void Start()
    {
        
        LoadssetBundleVersion();
        ApplicationVersion.text ="에플리케이션 버전:"+ Application.version;
        WriteButton.onClick.AddListener(() =>
        {
           
            
            string ApplicationVersion = "-Application;"+Application.version;
          for(int n = 0; n < VersionCheckers.Count; n++)
            {
                ApplicationVersion= MakeVersionText(ApplicationVersion, FileName[n], VersionCheckers[n].Version);
            }
            File.WriteAllText(assetBundleDir + "\\ApplicationVersion.txt", ApplicationVersion);
            Debug.Log(assetBundleDir + "\\ApplicationVersion.txt" + "생성완료\n");
        });
    }
    static string MakeVersionText(string text, string bundlename,string version)
    {

            text += "\n";
            
            text += $"-{bundlename}:{version}";
            return text;

      
    }
     void LoadssetBundleVersion()
    {
        for(int n=0;n< VersionCheckers.Count; n++)
        {
           
                string assetBundleDir = Directory.GetCurrentDirectory() + "\\Assets\\StreamingAssets" + "\\Assets";
                AssetBundle bundle = AssetBundle.LoadFromFile(assetBundleDir + $"\\{FileName[n]}");
                TextAsset t = bundle.LoadAsset("BundleVersion") as TextAsset;
                string s = t.text.Substring(9);
                VersionCheckers[n].setText(FileName[n], FilePaths[n], s);
          
        }
      
    }
 
}

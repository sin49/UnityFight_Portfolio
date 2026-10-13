using UnityEditor;
using System.IO;
using UnityEngine;

public class VersionEditor
{
    [UnityEditor.MenuItem("Version/Make Version")]
    static void MakeVersion()
    {
 


        string assetBundleDir = Directory.GetCurrentDirectory() + "\\Assets\\StreamingAssets\\"+"\\Assetbundle";

        if (!Directory.Exists(assetBundleDir))
            Directory.CreateDirectory(assetBundleDir);

        BuildPipeline.BuildAssetBundles(assetBundleDir, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    
      

    }
 
}

using UnityEngine;
using UnityEditor;
using System.IO;
public class AssetBundleMaker
{
    [UnityEditor.MenuItem("Assets/Build All AssetBundle")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDir = Directory.GetCurrentDirectory() +"\\Assets\\StreamingAssets\\Assets";

        if (!Directory.Exists(assetBundleDir))
            Directory.CreateDirectory(assetBundleDir);

        BuildPipeline.BuildAssetBundles(assetBundleDir, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }
  

}

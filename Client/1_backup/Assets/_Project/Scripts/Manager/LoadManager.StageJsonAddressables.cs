using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class LoadManager
{
    // Share immutable source TextAssets, never mutable stage/reward/mission data.
    private static readonly Dictionary<string, AsyncOperationHandle<TextAsset>>
        StageJsonHandles = new Dictionary<string, AsyncOperationHandle<TextAsset>>(StringComparer.Ordinal);
    private static bool stageJsonQuitSubscribed;

    private IEnumerator LoadStageJsonAddressableAsync(string jsonAddress,
        Action<StageInformation> action, float progressWeight = 2f)
    {
        if (!stageJsonQuitSubscribed)
        {
            Application.quitting += ReleaseStageJsonSessionHandles;
            stageJsonQuitSubscribed = true;
        }

        AsyncOperationHandle<TextAsset> handle;
        if (!StageJsonHandles.TryGetValue(jsonAddress, out handle) || !handle.IsValid())
        {
            handle = Addressables.LoadAssetAsync<TextAsset>(jsonAddress);
            StageJsonHandles[jsonAddress] = handle;
        }

        float startProgress = processingcount;
        while (handle.IsValid() && !handle.IsDone)
        {
            Loadingprogress = startProgress + handle.PercentComplete * progressWeight;
            yield return null;
        }
        processingcount += progressWeight;
        Loadingprogress = processingcount;

        StageInformation information = null;
        string error = "Invalid operation or missing Stage JSON result";
        if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
        {
            try
            {
                // Fresh parse for every callback, even on a shared handle cache hit.
                information = convertTextasset<StageInformation>(handle.Result);
            }
            catch (Exception exception)
            {
                error = exception.Message;
            }
        }
        else if (handle.IsValid() && handle.OperationException != null)
        {
            error = handle.OperationException.Message;
        }

        if (information == null)
        {
            AsyncOperationHandle<TextAsset> owned;
            if (StageJsonHandles.TryGetValue(jsonAddress, out owned) && owned.Equals(handle))
            {
                StageJsonHandles.Remove(jsonAddress);
                if (handle.IsValid()) Addressables.Release(handle);
            }
            Debug.LogError("Stage JSON Addressables load failed: " + jsonAddress + ". " + error);
            // stageinfoUI assumes a non-null result. Preserve the legacy failure
            // contract: do not invoke its success callback when loading/parsing fails.
            yield break;
        }

        action.Invoke(information);
    }

    private IEnumerator LoadAllStageJsonsAddressableAsync(Action<List<StageInformation>> action)
    {
        var contents = new List<StageContentMapping.Content>(StageContentMapping.AllContents);
        MaxLoadingprogress += contents.Count + 1;
        processingcount++;
        Loadingprogress = processingcount;
        var stages = new List<StageInformation>(contents.Count);
        foreach (var content in contents)
        {
            StageInformation stage = null;
            yield return LoadStageJsonAddressableAsync(content.JsonAddress, info => stage = info, 1f);
            if (stage == null)
            {
                // Keep the existing account update callback atomic across the catalog.
                Debug.LogError("Stage JSON preparation failed. Stage list update cancelled.");
                yield break;
            }
            stages.Add(stage);
        }
        action.Invoke(stages);
    }

    private static void ReleaseStageJsonSessionHandles()
    {
        Application.quitting -= ReleaseStageJsonSessionHandles;
        stageJsonQuitSubscribed = false;
        foreach (var handle in StageJsonHandles.Values)
            if (handle.IsValid()) Addressables.Release(handle);
        StageJsonHandles.Clear();
    }
}

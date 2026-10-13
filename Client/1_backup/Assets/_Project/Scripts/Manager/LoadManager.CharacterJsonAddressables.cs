using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class LoadManager
{
    // Follow the existing prefab session-ownership pattern, with a typed JSON cache.
    // Cache source assets, not mutable CharacterInformation skill/progress objects.
    private static readonly Dictionary<string, AsyncOperationHandle<TextAsset>>
        CharacterJsonHandles = new Dictionary<string, AsyncOperationHandle<TextAsset>>(StringComparer.Ordinal);
    private static bool characterJsonQuitSubscribed;

    public string ReturnChrDataName(CharacterData data)
    {
        string address = CharacterContentMapping.Get(data.id).JsonAddress;
        AsyncOperationHandle<TextAsset> handle;
        if (!CharacterJsonHandles.TryGetValue(address, out handle) || !handle.IsValid() ||
            !handle.IsDone || handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
            throw new InvalidOperationException("Character JSON is not prepared for ID " + data.id +
                " (address " + address + "). Load it asynchronously before requesting its name.");
        return convertTextasset<CharacterInformation>(handle.Result).name;
    }

    private IEnumerator LoadCharacterJsonAddressableAsync(string jsonAddress,
        Action<CharacterInformation> action, float progressWeight = 2f)
    {
        if (!characterJsonQuitSubscribed)
        {
            Application.quitting += ReleaseCharacterJsonSessionHandles;
            characterJsonQuitSubscribed = true;
        }

        AsyncOperationHandle<TextAsset> handle;
        if (!CharacterJsonHandles.TryGetValue(jsonAddress, out handle) || !handle.IsValid())
        {
            handle = Addressables.LoadAssetAsync<TextAsset>(jsonAddress);
            CharacterJsonHandles[jsonAddress] = handle;
        }
        float startProgress = processingcount;
        while (handle.IsValid() && !handle.IsDone)
        {
            Loadingprogress = startProgress + handle.PercentComplete * progressWeight;
            yield return null;
        }
        processingcount += progressWeight;
        Loadingprogress = processingcount;

        CharacterInformation information = null;
        string error = "Invalid operation or missing Character JSON result";
        if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
        {
            try
            {
                // Fresh object for every callback, including cache hits and duplicate slots.
                information = convertTextasset<CharacterInformation>(handle.Result);
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
            if (CharacterJsonHandles.TryGetValue(jsonAddress, out owned) && owned.Equals(handle))
            {
                CharacterJsonHandles.Remove(jsonAddress);
                if (handle.IsValid()) Addressables.Release(handle);
            }
            Debug.LogError("Character JSON Addressables load failed: " + jsonAddress + ". " + error);
            action.Invoke(null);
            yield break;
        }
        action.Invoke(information);
    }

    private IEnumerator LoadCharacterJsonsAddressableAsync(List<string> jsonAddresses,
        Action<List<CharacterInformation>> action)
    {
        processingcount++;
        Loadingprogress = processingcount;
        var information = new List<CharacterInformation>(jsonAddresses.Count);
        foreach (string address in jsonAddresses)
            yield return LoadCharacterJsonAddressableAsync(address, data => information.Add(data), 1f);
        action.Invoke(information);
    }

    private static void ReleaseCharacterJsonSessionHandles()
    {
        Application.quitting -= ReleaseCharacterJsonSessionHandles;
        characterJsonQuitSubscribed = false;
        foreach (var handle in CharacterJsonHandles.Values)
            if (handle.IsValid()) Addressables.Release(handle);
        CharacterJsonHandles.Clear();
    }
}

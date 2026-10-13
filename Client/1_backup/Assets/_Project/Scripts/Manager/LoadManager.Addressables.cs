using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class LoadManager
{
    // The prefab-only callback cannot report when its later clones stop using
    // dependencies. One session-owned reference per address is deliberately kept
    // across menus/scenes and LoadManager destruction, until application shutdown.
    private static readonly Dictionary<string, AsyncOperationHandle<GameObject>>
        CharacterPrefabHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>(StringComparer.Ordinal);
    private static bool characterPrefabQuitSubscribed;

    private IEnumerator LoadCharacterPrefabAddressableAsync(string prefabAddress, Action<GameObject> action, float progressWeight = 2f)
    {
        if (!characterPrefabQuitSubscribed)
        {
            Application.quitting += ReleaseCharacterPrefabSessionHandles;
            characterPrefabQuitSubscribed = true;
        }

        AsyncOperationHandle<GameObject> handle;
        if (!CharacterPrefabHandles.TryGetValue(prefabAddress, out handle) || !handle.IsValid())
        {
            handle = Addressables.LoadAssetAsync<GameObject>(prefabAddress);
            // Store before yielding so interrupted requests still have an owner.
            CharacterPrefabHandles[prefabAddress] = handle;
        }

        float startProgress = processingcount;
        // Poll explicitly: concurrent queue runners can share a failed operation
        // whose first observer releases it. Never inspect an invalidated handle.
        while (handle.IsValid() && !handle.IsDone)
        {
            Loadingprogress = startProgress + handle.PercentComplete * progressWeight;
            yield return null;
        }
        processingcount += progressWeight;
        Loadingprogress = processingcount;

        if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
        {
            string reason = handle.IsValid() && handle.OperationException != null
                ? handle.OperationException.Message : "Invalid operation or missing prefab result";
            AsyncOperationHandle<GameObject> owned;
            if (CharacterPrefabHandles.TryGetValue(prefabAddress, out owned) && owned.Equals(handle))
            {
                CharacterPrefabHandles.Remove(prefabAddress);
                if (handle.IsValid()) Addressables.Release(handle);
            }
            Debug.LogError("Character prefab Addressables load failed: " + prefabAddress + ". " + reason);
            action.Invoke(null);
            yield break;
        }

        // Return the prefab asset, never an instance. The cache retains the handle
        // even if a consumer callback throws or its queue runner is destroyed.
        action.Invoke(handle.Result);
    }

    private IEnumerator LoadCharacterPrefabsAddressableAsync(List<string> prefabAddresses, Action<List<GameObject>> action)
    {
        // Preserve the existing batch budget: one setup unit plus one per slot.
        processingcount++;
        Loadingprogress = processingcount;
        var prefabs = new List<GameObject>(prefabAddresses.Count);
        foreach (string address in prefabAddresses)
        {
            // Reuse single-load ownership, cache and failure handling. Sequential
            // completion preserves duplicate entries and null failure slots.
            yield return LoadCharacterPrefabAddressableAsync(address, prefab => prefabs.Add(prefab), 1f);
        }
        action.Invoke(prefabs);
    }

    private static void ReleaseCharacterPrefabSessionHandles()
    {
        Application.quitting -= ReleaseCharacterPrefabSessionHandles;
        characterPrefabQuitSubscribed = false;
        foreach (var handle in CharacterPrefabHandles.Values)
            if (handle.IsValid()) Addressables.Release(handle);
        CharacterPrefabHandles.Clear();
    }
}

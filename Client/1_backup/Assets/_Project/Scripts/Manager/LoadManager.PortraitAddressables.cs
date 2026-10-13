using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class LoadManager
{
    // UI keeps Sprite references across screens without reporting their lifetime.
    // Follow the existing prefab/JSON policy: one owned handle per session/address.
    private static readonly Dictionary<string, AsyncOperationHandle<Sprite>>
        CharacterPortraitHandles = new Dictionary<string, AsyncOperationHandle<Sprite>>(StringComparer.Ordinal);
    private static bool characterPortraitQuitSubscribed;

    private IEnumerator LoadCharacterPortraitAddressableAsync(string portraitAddress, Action<Sprite> action, float progressWeight = 2f)
    {
        if (!characterPortraitQuitSubscribed)
        {
            Application.quitting += ReleaseCharacterPortraitSessionHandles;
            characterPortraitQuitSubscribed = true;
        }

        AsyncOperationHandle<Sprite> handle;
        if (!CharacterPortraitHandles.TryGetValue(portraitAddress, out handle) || !handle.IsValid())
        {
            handle = Addressables.LoadAssetAsync<Sprite>(portraitAddress);
            // Own in-flight operations before yielding, including interrupted loads.
            CharacterPortraitHandles[portraitAddress] = handle;
        }

        float startProgress = processingcount;
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
                ? handle.OperationException.Message : "Invalid operation or missing portrait result";
            AsyncOperationHandle<Sprite> owned;
            if (CharacterPortraitHandles.TryGetValue(portraitAddress, out owned) && owned.Equals(handle))
            {
                CharacterPortraitHandles.Remove(portraitAddress);
                if (handle.IsValid()) Addressables.Release(handle);
            }
            Debug.LogError("Character portrait Addressables load failed: " + portraitAddress + ". " + reason);
            // Preserve LoadSpriteAsync: no success callback for a missing Sprite.
            yield break;
        }

        // Imported Single Sprites already have the full image, centered pivot and
        // 100 PPU used by the old Sprite.Create path. UI receives the Sprite asset.
        CharacterSpriteDictionary[portraitAddress] = handle.Result;
        action.Invoke(handle.Result);
    }

    private IEnumerator LoadCharacterPortraitsAddressableAsync(List<string> portraitAddresses, Action<List<Sprite>> action)
    {
        processingcount++;
        Loadingprogress = processingcount;
        var portraits = new List<Sprite>(portraitAddresses.Count);
        foreach (string address in portraitAddresses)
        {
            // Successful entries keep input order/duplicates. Like LoadSpritesAsync,
            // missing portraits are omitted; this differs from the prefab batch API.
            yield return LoadCharacterPortraitAddressableAsync(address, sprite => portraits.Add(sprite), 1f);
        }
        action.Invoke(portraits);
    }

    private IEnumerator LoadAllCharacterPortraitsAddressableAsync()
    {
        var addresses = new List<string>();
        foreach (var content in CharacterContentMapping.AllContents)
            addresses.Add(content.PortraitAddress);
        MaxLoadingprogress += addresses.Count + 1;
        // Populate the same synchronous UI dictionary before loadPlayerSprites.
        // A recreated manager also restores it from the session-owned handles.
        yield return LoadCharacterPortraitsAddressableAsync(addresses, portraits => { });
    }

    private static void ReleaseCharacterPortraitSessionHandles()
    {
        Application.quitting -= ReleaseCharacterPortraitSessionHandles;
        characterPortraitQuitSubscribed = false;
        foreach (var handle in CharacterPortraitHandles.Values)
            if (handle.IsValid()) Addressables.Release(handle);
        CharacterPortraitHandles.Clear();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class LoadManager
{
    // Explicitly resolve the one existing SFX; never infer an address from a name.
    private const string CharacterHitSfxAddress = "character/sfx/Hit";
    private static readonly Dictionary<string, AsyncOperationHandle<AudioClip>>
        CharacterSfxHandles = new Dictionary<string, AsyncOperationHandle<AudioClip>>(StringComparer.Ordinal);
    private static bool characterSfxQuitSubscribed;

    private IEnumerator LoadCharacterSfxAddressableAsync(string sfxName, Action<AudioClip> action)
    {
        if (!string.Equals(sfxName, "Hit", StringComparison.OrdinalIgnoreCase))
        {
            processingcount += 2f;
            Loadingprogress = processingcount;
            Debug.LogError("No Character SFX address is registered for name: " + sfxName);
            action.Invoke(null);
            yield break;
        }

        string address = CharacterHitSfxAddress;
        if (!characterSfxQuitSubscribed)
        {
            Application.quitting += ReleaseCharacterSfxSessionHandles;
            characterSfxQuitSubscribed = true;
        }

        AsyncOperationHandle<AudioClip> handle;
        if (!CharacterSfxHandles.TryGetValue(address, out handle) || !handle.IsValid())
        {
            handle = Addressables.LoadAssetAsync<AudioClip>(address);
            // Store before yielding so interrupted/concurrent requests keep an owner.
            CharacterSfxHandles[address] = handle;
        }

        float startProgress = processingcount;
        while (handle.IsValid() && !handle.IsDone)
        {
            Loadingprogress = startProgress + handle.PercentComplete * 2f;
            yield return null;
        }
        processingcount += 2f;
        Loadingprogress = processingcount;

        if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
        {
            string reason = handle.IsValid() && handle.OperationException != null
                ? handle.OperationException.Message : "Invalid operation or missing Character SFX result";
            AsyncOperationHandle<AudioClip> owned;
            if (CharacterSfxHandles.TryGetValue(address, out owned) && owned.Equals(handle))
            {
                CharacterSfxHandles.Remove(address);
                if (handle.IsValid()) Addressables.Release(handle);
            }
            Debug.LogError("Character SFX Addressables load failed: " + address + ". " + reason);
            action.Invoke(null);
            yield break;
        }

        // Consumers retain the AudioClip; the session cache retains its handle.
        action.Invoke(handle.Result);
    }

    private static void ReleaseCharacterSfxSessionHandles()
    {
        Application.quitting -= ReleaseCharacterSfxSessionHandles;
        characterSfxQuitSubscribed = false;
        foreach (var handle in CharacterSfxHandles.Values)
            if (handle.IsValid()) Addressables.Release(handle);
        CharacterSfxHandles.Clear();
    }
}

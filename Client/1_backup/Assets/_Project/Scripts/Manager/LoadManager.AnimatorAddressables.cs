using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class LoadManager
{
    // Controllers and their clips can be used by characters across scenes.
    // Keep one owned handle per address until shutdown, like the other loaders.
    private static readonly Dictionary<string, AsyncOperationHandle<RuntimeAnimatorController>>
        CharacterAnimatorHandles = new Dictionary<string, AsyncOperationHandle<RuntimeAnimatorController>>(StringComparer.Ordinal);
    private static bool characterAnimatorQuitSubscribed;

    private IEnumerator LoadCharacterAnimatorAddressableAsync(string animatorAddress,
        Action<RuntimeAnimatorController> action, float progressWeight = 2f)
    {
        // P04 deliberately has no controller. Preserve its callback/null slot
        // without creating a key, operation, fallback or failure log.
        if (animatorAddress == null)
        {
            processingcount += progressWeight;
            Loadingprogress = processingcount;
            action.Invoke(null);
            yield break;
        }

        if (!characterAnimatorQuitSubscribed)
        {
            Application.quitting += ReleaseCharacterAnimatorSessionHandles;
            characterAnimatorQuitSubscribed = true;
        }

        AsyncOperationHandle<RuntimeAnimatorController> handle;
        if (!CharacterAnimatorHandles.TryGetValue(animatorAddress, out handle) || !handle.IsValid())
        {
            handle = Addressables.LoadAssetAsync<RuntimeAnimatorController>(animatorAddress);
            // Cache before yielding so interrupted/concurrent requests share ownership.
            CharacterAnimatorHandles[animatorAddress] = handle;
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
                ? handle.OperationException.Message : "Invalid operation or missing animator result";
            AsyncOperationHandle<RuntimeAnimatorController> owned;
            if (CharacterAnimatorHandles.TryGetValue(animatorAddress, out owned) && owned.Equals(handle))
            {
                CharacterAnimatorHandles.Remove(animatorAddress);
                if (handle.IsValid()) Addressables.Release(handle);
            }
            Debug.LogError("Character animator Addressables load failed: " + animatorAddress + ". " + reason);
            // Battle consumers combine prefab/master/controller by index.
            action.Invoke(null);
            yield break;
        }

        // Return the controller asset; callbacks do not transfer handle ownership.
        action.Invoke(handle.Result);
    }

    private IEnumerator LoadCharacterAnimatorsAddressableAsync(List<string> animatorAddresses,
        Action<List<RuntimeAnimatorController>> action)
    {
        processingcount++;
        Loadingprogress = processingcount;
        var controllers = new List<RuntimeAnimatorController>(animatorAddresses.Count);
        foreach (string address in animatorAddresses)
        {
            // Reuse the single loader. One result per input preserves order,
            // duplicates, intentional unsupported slots and failed-load nulls.
            yield return LoadCharacterAnimatorAddressableAsync(address, controller => controllers.Add(controller), 1f);
        }
        action.Invoke(controllers);
    }

    private static void ReleaseCharacterAnimatorSessionHandles()
    {
        Application.quitting -= ReleaseCharacterAnimatorSessionHandles;
        characterAnimatorQuitSubscribed = false;
        foreach (var handle in CharacterAnimatorHandles.Values)
            if (handle.IsValid()) Addressables.Release(handle);
        CharacterAnimatorHandles.Clear();
    }
}

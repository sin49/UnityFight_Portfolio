using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class LoadManager
{
    // Stage creates and pools ordinary clones without reporting their lifetime.
    // Follow Character Prefab ownership: one handle per address until shutdown.
    private static readonly Dictionary<string, AsyncOperationHandle<GameObject>>
        TerrainPrefabHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>(StringComparer.Ordinal);
    private static bool terrainPrefabQuitSubscribed;

    private IEnumerator LoadTerrainPrefabAddressableAsync(string terrainName, Action<GameObject> action)
    {
        // The old AssetBundle.LoadAssetAsync rejected a null asset name. Keep this
        // queued failure instead of making the existing null-Stage issue a fallback.
        if (terrainName == null)
            throw new NullReferenceException("The input asset name cannot be null.");

        string prefabAddress = TerrainContentMapping.Get(terrainName).PrefabAddress;
        if (!terrainPrefabQuitSubscribed)
        {
            Application.quitting += ReleaseTerrainPrefabSessionHandles;
            terrainPrefabQuitSubscribed = true;
        }

        AsyncOperationHandle<GameObject> handle;
        if (!TerrainPrefabHandles.TryGetValue(prefabAddress, out handle) || !handle.IsValid())
        {
            handle = Addressables.LoadAssetAsync<GameObject>(prefabAddress);
            // Own the operation before yielding, including interrupted requests.
            TerrainPrefabHandles[prefabAddress] = handle;
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
                ? handle.OperationException.Message : "Invalid operation or missing terrain prefab result";
            AsyncOperationHandle<GameObject> owned;
            if (TerrainPrefabHandles.TryGetValue(prefabAddress, out owned) && owned.Equals(handle))
            {
                TerrainPrefabHandles.Remove(prefabAddress);
                if (handle.IsValid()) Addressables.Release(handle);
            }
            Debug.LogError("Terrain prefab Addressables load failed: " + prefabAddress + ". " + reason);
            action.Invoke(null);
            yield break;
        }

        // Return the prefab asset, not an instance. Keep dependencies alive for
        // Stage's initial Instantiate and every manually cloned pooled tile.
        action.Invoke(handle.Result);
    }

    private static void ReleaseTerrainPrefabSessionHandles()
    {
        Application.quitting -= ReleaseTerrainPrefabSessionHandles;
        terrainPrefabQuitSubscribed = false;
        foreach (var handle in TerrainPrefabHandles.Values)
            if (handle.IsValid()) Addressables.Release(handle);
        TerrainPrefabHandles.Clear();
    }
}

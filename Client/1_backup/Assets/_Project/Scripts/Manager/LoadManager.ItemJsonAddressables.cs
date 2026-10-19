using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class LoadManager
{
    // Share source TextAssets only. ItemData is mutable, notably its itemSprite.
    private static readonly Dictionary<string, AsyncOperationHandle<TextAsset>>
        ItemJsonHandles = new Dictionary<string, AsyncOperationHandle<TextAsset>>(StringComparer.Ordinal);
    private static bool itemJsonQuitSubscribed;

    private IEnumerator LoadItemJsonAddressableAsync(string itemId, Action<ItemData> action, float progressWeight = 2f)
    {
        string address = ItemContentMapping.Get(itemId).JsonAddress;
        if (!itemJsonQuitSubscribed)
        {
            Application.quitting += ReleaseItemJsonSessionHandles;
            itemJsonQuitSubscribed = true;
        }

        AsyncOperationHandle<TextAsset> handle;
        if (!ItemJsonHandles.TryGetValue(address, out handle) || !handle.IsValid())
        {
            handle = Addressables.LoadAssetAsync<TextAsset>(address);
            ItemJsonHandles[address] = handle;
        }
        float startProgress = processingcount;
        while (handle.IsValid() && !handle.IsDone)
        {
            Loadingprogress = startProgress + handle.PercentComplete * progressWeight;
            yield return null;
        }
        processingcount += progressWeight;
        Loadingprogress = processingcount;

        ItemData information = null;
        string error = "Invalid operation or missing Item JSON result";
        if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
        {
            try
            {
                // A fresh graph for each request/cache hit, never the dictionary object.
                information = convertTextasset<ItemData>(handle.Result);
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
            if (ItemJsonHandles.TryGetValue(address, out owned) && owned.Equals(handle))
            {
                ItemJsonHandles.Remove(address);
                if (handle.IsValid()) Addressables.Release(handle);
            }
            Debug.LogError("Item JSON Addressables load failed: " + address + ". " + error);
            action.Invoke(null);
            yield break;
        }
        action.Invoke(information);
    }

    private IEnumerator LoadItemJsonsAddressableAsync(List<string> itemIds, Action<List<ItemData>> action,
        float progressBudget = 2f)
    {
        var data = new List<ItemData>(itemIds.Count);
        if (itemIds.Count == 0)
        {
            processingcount += progressBudget;
            Loadingprogress = processingcount;
        }
        else
        {
            float weight = progressBudget / itemIds.Count;
            foreach (string id in itemIds)
                yield return LoadItemJsonAddressableAsync(id, item => data.Add(item), weight);
        }
        // Retain input order, duplicate slots and null failures for JSON consumers.
        action.Invoke(data);
    }

    private IEnumerator LoadItemJsonWithLegacySpriteAsync(string itemId, Action<ItemData> action)
    {
        ItemData data = null;
        // Existing combined API budget is used by the Sprite helper.
        yield return LoadItemJsonAddressableAsync(itemId, item => data = item, 0f);
        yield return StartCoroutine(LoadItemSpriteAddressableAsync(itemId,
            sprite => { if (data != null) data.itemSprite = sprite; }));
        action.Invoke(data);
    }

    private IEnumerator LoadItemsJsonWithLegacySpritesAsync(List<string> itemIds, Action<List<ItemData>> action)
    {
        List<ItemData> data = null;
        yield return LoadItemJsonsAddressableAsync(itemIds, items => data = items, 0f);
        yield return StartCoroutine(LoadItemSpritesAddressableAsync(itemIds, sprites =>
        {
            // JSON failures retain slots. Do not dereference a failed JSON result.
            // Keep the existing Sprite batch's success-only list contract unchanged.
            for (int n = 0; n < sprites.Count; n++)
                if (data[n] != null) data[n].itemSprite = sprites[n];
        }));
        action.Invoke(data);
    }

    private IEnumerator LoadAllItemJsonWithLegacySpritesAsync()
    {
        itemDictionary = new Dictionary<string, ItemData>();
        var contents = new List<ItemContentMapping.Content>(ItemContentMapping.AllContents);
        // One JSON unit plus a two-unit Sprite load per catalog entry.
        MaxLoadingprogress += contents.Count * 3f;
        foreach (var content in contents)
        {
            ItemData item = null;
            yield return LoadItemJsonAddressableAsync(content.ItemId, data => item = data, 1f);
            if (item == null)
            {
                Debug.LogError("Item JSON preload failed. Item dictionary preparation stopped.");
                yield break;
            }
            yield return StartCoroutine(LoadItemSpriteAddressableAsync(item.id, sprite => item.itemSprite = sprite));
            itemDictionary.Add(item.id, item);
            Loadingprogress = processingcount;
        }
    }

    private static void ReleaseItemJsonSessionHandles()
    {
        Application.quitting -= ReleaseItemJsonSessionHandles;
        itemJsonQuitSubscribed = false;
        foreach (var handle in ItemJsonHandles.Values)
            if (handle.IsValid()) Addressables.Release(handle);
        ItemJsonHandles.Clear();
    }
}

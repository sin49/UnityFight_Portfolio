using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class LoadManager
{
    private static readonly Dictionary<string, AsyncOperationHandle<Texture2D>>
        ItemSpriteTextureHandles = new Dictionary<string, AsyncOperationHandle<Texture2D>>(StringComparer.Ordinal);
    // These are runtime Sprites, not imported Addressables Sprite assets.
    private static readonly Dictionary<string, Sprite>
        ItemSessionSprites = new Dictionary<string, Sprite>(StringComparer.Ordinal);
    private static bool itemSpriteQuitSubscribed;

    private IEnumerator LoadItemSpriteAddressableAsync(string itemId, Action<Sprite> action, float progressWeight = 2f)
    {
        string address = ItemContentMapping.Get(itemId).SpriteAddress;
        if (!itemSpriteQuitSubscribed)
        {
            Application.quitting += ReleaseItemSpriteSessionHandles;
            itemSpriteQuitSubscribed = true;
        }

        AsyncOperationHandle<Texture2D> handle;
        if (!ItemSpriteTextureHandles.TryGetValue(address, out handle) || !handle.IsValid())
        {
            handle = Addressables.LoadAssetAsync<Texture2D>(address);
            ItemSpriteTextureHandles[address] = handle;
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
                ? handle.OperationException.Message : "Invalid operation or missing Item texture";
            AsyncOperationHandle<Texture2D> owned;
            if (ItemSpriteTextureHandles.TryGetValue(address, out owned) && owned.Equals(handle))
            {
                ItemSpriteTextureHandles.Remove(address);
                if (handle.IsValid()) Addressables.Release(handle);
            }
            Debug.LogError("Item Sprite Addressables load failed: " + address + ". " + reason);
            // Legacy single did not invoke its callback when the texture was missing.
            yield break;
        }

        Sprite sprite;
        if (!ItemSessionSprites.TryGetValue(address, out sprite) || sprite == null)
        {
            Texture2D texture = handle.Result;
            // Keep the exact Legacy Sprite.Create overload and arguments.
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            ItemSessionSprites[address] = sprite;
        }
        action.Invoke(sprite);
    }

    private IEnumerator LoadItemSpritesAddressableAsync(List<string> itemIds, Action<List<Sprite>> action)
    {
        processingcount++;
        Loadingprogress = processingcount;
        var sprites = new List<Sprite>(itemIds.Count);
        foreach (string itemId in itemIds)
        {
            // Preserve successful input order/duplicates; missing textures are omitted,
            // as in Legacy LoadSpritesAsync. Do not repair the combined batch contract here.
            yield return LoadItemSpriteAddressableAsync(itemId, sprite => sprites.Add(sprite), 1f);
        }
        action.Invoke(sprites);
    }

    private static void ReleaseItemSpriteSessionHandles()
    {
        Application.quitting -= ReleaseItemSpriteSessionHandles;
        itemSpriteQuitSubscribed = false;
        // Runtime Sprite ownership is separate from the Texture Addressables handle.
        foreach (var sprite in ItemSessionSprites.Values)
            if (sprite != null) UnityEngine.Object.Destroy(sprite);
        ItemSessionSprites.Clear();
        foreach (var handle in ItemSpriteTextureHandles.Values)
            if (handle.IsValid()) Addressables.Release(handle);
        ItemSpriteTextureHandles.Clear();
    }
}

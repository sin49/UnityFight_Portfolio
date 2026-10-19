using System;
using System.Collections.Generic;

// Item identities, physical JSON names and Addressables keys are independent.
public static class ItemContentMapping
{
    public sealed class Content
    {
        public readonly string ItemId;
        public readonly string JsonName;
        public readonly string JsonAddress;
        public readonly string SpriteName;
        public readonly string SpriteAddress;

        internal Content(string itemId, string jsonName, string jsonAddress, string spriteName, string spriteAddress)
        {
            ItemId = itemId;
            JsonName = jsonName;
            JsonAddress = jsonAddress;
            SpriteName = spriteName;
            SpriteAddress = spriteAddress;
        }
    }

    private static readonly Dictionary<string, Content> Entries =
        new Dictionary<string, Content>(StringComparer.OrdinalIgnoreCase)
        {
            { "I01", new Content("I01", "I01", "item/json/I01", "I01", "item/sprite/I01") },
            { "I02", new Content("I02", "I02", "item/json/I02", "I02", "item/sprite/I02") },
            { "I03", new Content("I03", "I03", "item/json/I03", "I03", "item/sprite/I03") },
            { "I04", new Content("I04", "I04", "item/json/I04", "I04", "item/sprite/I04") },
            { "I05", new Content("I05", "I05", "item/json/I05", "I05", "item/sprite/I05") }
        };

    internal static IEnumerable<Content> AllContents { get { return Entries.Values; } }

    public static Content Get(string itemId)
    {
        Content content;
        if (itemId == null || !Entries.TryGetValue(itemId, out content))
            throw new ArgumentException("No item JSON content mapping for ID: " + itemId, "itemId");
        return content;
    }
}

using System;
using System.Collections.Generic;

// Character IDs are logical identities. These names describe the existing content,
// including its original casing; they are not derived from an ID naming convention.
public static class CharacterContentMapping
{
    public sealed class Content
    {
        public readonly string CharacterId;
        public readonly string JsonName;
        public readonly string PrefabName;
        public readonly string PortraitName;
        public readonly string AnimatorName;
        // Addressables keys are logical namespaces, independent of physical names.
        public readonly string PrefabAddress;
        public readonly string JsonAddress;
        public readonly string PortraitAddress;
        public readonly string AnimatorAddress;
        // Preserve the existing fragment item lookup during the ID migration.
        public readonly string LegacyFragmentItemId;

        internal Content(string id, string json, string prefab, string portrait, string animator, string fragmentItem)
        {
            CharacterId = id;
            JsonName = json;
            PrefabName = prefab;
            PortraitName = portrait;
            AnimatorName = animator;
            PrefabAddress = "character/prefab/" + CharacterId;
            JsonAddress = "character/json/" + CharacterId;
            PortraitAddress = "character/portrait/" + CharacterId;
            // Missing animator content stays unsupported; never invent an address.
            AnimatorAddress = animator == null ? null : "character/animator/" + CharacterId;
            LegacyFragmentItemId = fragmentItem;
        }
    }

    private static readonly Dictionary<string, Content> Entries =
        new Dictionary<string, Content>(StringComparer.OrdinalIgnoreCase)
        {
            { "p01", new Content("p01", "p01", "P01", "p01", "P01", "Ip01") },
            { "p02", new Content("p02", "P02", "P02", "P02", "p02", "Ip02") },
            { "p03", new Content("p03", "p03", "P03", "P03", "p03", "Ip03") },
            // No P04 animator exists. Null is intentional, not a fallback.
            { "p04", new Content("p04", "P04", "P04", "P04", null, "IP04") },
            { "e00", new Content("e00", "E00", "E00", "E00", "E00", "IE00") }
        };

    internal static IEnumerable<Content> AllContents { get { return Entries.Values; } }

    public static bool TryGet(string characterId, out Content content)
    {
        content = null;
        return characterId != null && Entries.TryGetValue(characterId, out content);
    }

    public static Content Get(string characterId)
    {
        Content content;
        if (!TryGet(characterId, out content))
            throw new ArgumentException("No character content mapping for ID: " + characterId, "characterId");
        return content;
    }

    // Only registered aliases are converted. Empty/sentinel/unknown historical IDs
    // are preserved so this migration does not silently invent character identities.
    public static string Canonicalize(string characterId)
    {
        Content content;
        return TryGet(characterId, out content) ? content.CharacterId : characterId;
    }

    public static List<string> Resolve(List<string> ids, Func<Content, string> select)
    {
        var names = new List<string>(ids.Count);
        foreach (string id in ids)
            names.Add(select(Get(id)));
        return names;
    }

    public static T NormalizeData<T>(T data)
    {
        var master = (object)data as CharacterInformation;
        if (master != null) master.id = Canonicalize(master.id);
        var character = (object)data as CharacterData;
        if (character != null) character.id = Canonicalize(character.id);
        var localAccount = (object)data as UserAccountData;
        if (localAccount != null) localAccount.NormalizeCharacterIds();
        var stage = (object)data as StageInformation;
        if (stage != null && stage.SpawnCharacterList != null)
            foreach (var spawn in stage.SpawnCharacterList)
                if (spawn != null) spawn.id = Canonicalize(spawn.id);
        var account = (object)data as UserAccountDataNetworkMember;
        if (account != null)
        {
            // Preflight before mutating the DTO: aliases must not silently merge progress.
            var ids = new HashSet<string>(StringComparer.Ordinal);
            if (account.Characterlist != null)
                foreach (var owned in account.Characterlist)
                    if (owned != null && !ids.Add(Canonicalize(owned.id)))
                        throw new InvalidOperationException("Duplicate logical character ID: " + Canonicalize(owned.id));
            account.StarChracter = Canonicalize(account.StarChracter);
            if (account.Characterlist != null)
                foreach (var owned in account.Characterlist)
                    if (owned != null) owned.id = Canonicalize(owned.id);
        }
        return data;
    }
}

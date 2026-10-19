using System;
using System.Collections.Generic;

// Selection values, physical prefab names and addresses are independent data.
public static class TerrainContentMapping
{
    public sealed class Content
    {
        public readonly string TerrainName;
        public readonly string PrefabName;
        public readonly string PrefabAddress;

        internal Content(string terrainName, string prefabName, string prefabAddress)
        {
            TerrainName = terrainName;
            PrefabName = prefabName;
            PrefabAddress = prefabAddress;
        }
    }

    private static readonly Dictionary<string, Content> Entries =
        new Dictionary<string, Content>(StringComparer.Ordinal)
        {
            { "Main0", new Content("Main0", "Main0", "terrain/prefab/Main0") }
        };

    public static Content Get(string terrainName)
    {
        Content content;
        if (terrainName == null || !Entries.TryGetValue(terrainName, out content))
            throw new ArgumentException("No terrain content mapping for name: " + terrainName, "terrainName");
        return content;
    }
}

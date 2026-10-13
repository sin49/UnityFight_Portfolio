using System;
using System.Collections.Generic;

// Stage IDs preserve their existing account/UI casing. Physical JSON names and
// Addressables keys are explicit entries, not inferred from a requested file name.
public static class StageContentMapping
{
    public sealed class Content
    {
        public readonly string StageId;
        public readonly string JsonName;
        public readonly string JsonAddress;

        internal Content(string stageId, string jsonName, string jsonAddress)
        {
            StageId = stageId;
            JsonName = jsonName;
            JsonAddress = jsonAddress;
        }
    }

    private static readonly Dictionary<string, Content> Entries =
        new Dictionary<string, Content>(StringComparer.Ordinal)
        {
            { "Main0-0", new Content("Main0-0", "Main0-0", "stage/json/Main0-0") },
            { "Main0-1", new Content("Main0-1", "Main0-1", "stage/json/Main0-1") }
        };

    internal static IEnumerable<Content> AllContents { get { return Entries.Values; } }

    public static Content Get(string stageId)
    {
        Content content;
        if (stageId == null || !Entries.TryGetValue(stageId, out content))
            throw new ArgumentException("No stage JSON content mapping for ID: " + stageId, "stageId");
        return content;
    }
}

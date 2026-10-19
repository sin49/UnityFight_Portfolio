using System;
using System.IO;

namespace Server.script
{
    // Shared file root for account/template and preserved shop helpers.
    // Default discovery starts from the executable, never the process working directory.
    internal static class ServerPaths
    {
        public static string Root { get; private set; }
        public static string AccountDirectory { get { return Path.Combine(Root, "DBprototype"); } }
        public static string AccountTemplate { get { return Path.Combine(AccountDirectory, "DataTemplet", "AccountTemplet.json"); } }

        public static void Initialize(string configuredRoot)
        {
            if (!string.IsNullOrWhiteSpace(configuredRoot))
            {
                if (!Path.IsPathRooted(configuredRoot))
                    throw new ArgumentException("--data-root must be an absolute path");
                Root = Path.GetFullPath(configuredRoot);
            }
            else
            {
                for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
                {
                    if (File.Exists(Path.Combine(directory.FullName, "DBprototype", "DataTemplet", "AccountTemplet.json")))
                    {
                        Root = directory.FullName;
                        break;
                    }
                }
            }
            if (Root == null || !File.Exists(AccountTemplate))
                throw new FileNotFoundException("Account template not found. Supply --data-root with DBprototype/DataTemplet/AccountTemplet.json.");
            Directory.CreateDirectory(AccountDirectory);
        }
    }
}

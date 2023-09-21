using System;
using System.IO;
using UnityEngine;

namespace Jira.Runtime.Utility
{
    public static class DirectoryPathFinder
    {
        public static bool FindDirectoryPathInProject(string directory, out string path)
        {
            var directories = Directory.GetDirectories(Path.GetDirectoryName(Application.dataPath) ?? Application.dataPath, directory, SearchOption.AllDirectories);

            if (directories.Length > 0)
            {
                path = directories[0];

                return true;
            }

            path = string.Empty;

            return false;
        }

        public static bool FindDirectoryPath(string directory, out string path)
        {
            var rootPath = GetRootPath();

            var directories = Directory.GetDirectories(rootPath, directory, SearchOption.AllDirectories);

            if (directories.Length > 0)
            {
                path = directories[0];

                return true;
            }

            path = string.Empty;

            return false;
        }

        private static string GetRootPath()
        {
#if UNITY_STANDALONE_WIN
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#elif UNITY_STANDALONE_OSX
        return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#elif UNITY_ANDROID
        return Application.persistentDataPath;
#elif UNITY_IOS
        return Application.persistentDataPath;
#else
        return Application.dataPath; // Default to dataPath for other platforms
#endif
        }
    }
}
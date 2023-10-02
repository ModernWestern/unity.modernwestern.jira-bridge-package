using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Jira.Editor.Utility
{
    using Runtime.Utility;

    public static class RockVRUtility
    {
        private const string RockVRFolder = "RockVR";

        private const string PackageFolder = "jira-bridge";

        public static void CreateAssemblyReference(Action<bool> complete)
        {
            FileImporter(complete, "AssemblyReference", File.Extension.asmref, PackageFolder, RockVRFolder, "Samples~");
        }

        public static void ModifyRockUtilsFile(Action<bool> complete)
        {
            FileImporter(complete, "Utils", File.Extension.cs, PackageFolder, RockVRFolder, "Video", "Scripts", "Utils");
        }

        private static void FileImporter(Action<bool> complete, string fileName, File.Extension extension, string source, string target, params string[] subFolders)
        {
            if (DirectoryPathFinder.FindDirectoryPathInProjectByPattern(source, out var sourcePath) && DirectoryPathFinder.FindDirectoryPathInProject(target, out var targetPath))
            {
#if UNITY_EDITOR
                Debug.Log($"SOURCE -> {sourcePath}, DESTINATION -> {targetPath}");
#endif
                try
                {
                    ArrayUtility.Add(ref subFolders, fileName);

                    System.IO.File.Copy(subFolders.Aggregate(sourcePath, Path.Combine), Path.ChangeExtension(Path.Combine(targetPath, fileName), extension.Get()), true);

                    AssetDatabase.Refresh();

                    complete?.Invoke(true);
                }
                catch (Exception e)
                {
                    complete?.Invoke(false);
#if UNITY_EDITOR
                    Debug.LogError(e);
#else
                        _ = e;
#endif
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Package not found");
#endif
                complete?.Invoke(false);
            }
        }
    }
}
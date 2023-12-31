﻿using System;
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

        public static void CreateAssemblyReference(Action<bool> complete = null)
        {
            FileImporter(complete, "AssemblyReference", File.Extension.asmref, PackageFolder, RockVRFolder, (CreateArray("Samples~"), null));
        }

        public static void ModifyRockUtilsFile(Action<bool> complete = null)
        {
            FileImporter(complete, "Utils", File.Extension.cs, PackageFolder, RockVRFolder, (CreateArray("Samples~"), CreateArray("Video", "Scripts", "Utils")));
        }

        #region HELPERS

        private static string[] CreateArray(params string[] paths) => paths;

        private static void FileImporter(Action<bool> complete, string fileName, File.Extension extension, string source, string target, (string[] source, string[] target) subFolders)
        {
            if (DirectoryPathFinder.FindDirectoryPathInProjectByPattern(source, out var sourcePath) && DirectoryPathFinder.FindDirectoryPathInProject(target, out var targetPath))
            {
#if UNITY_EDITOR
                Debug.Log($"SOURCE -> {sourcePath} : DESTINATION -> {targetPath}");
#endif
                try
                {
                    ArrayUtility.Add(ref subFolders.source, fileName);

                    var sourceFile = subFolders.source?.Aggregate(sourcePath, Path.Combine);

                    var targetFile = subFolders.target?.Aggregate(targetPath, Path.Combine);
#if UNITY_EDITOR
                    Debug.Log($"SOURCE FILE -> {sourceFile ?? sourcePath} : DESTINATION FILE -> {targetFile ?? targetPath}");
#endif
                    System.IO.File.Copy(sourceFile ?? sourcePath, Path.ChangeExtension(Path.Combine(targetFile ?? targetPath, fileName), extension.Get()), true);

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

        #endregion
    }
}
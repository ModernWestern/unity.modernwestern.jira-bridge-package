using System;
using System.IO;
using UnityEngine;
using Jira.Runtime.Utility;
using File = Jira.Runtime.Utility.File;

namespace Jira.Editor.Utility
{
    public static class RockVRUtility
    {
        private const string Target = "RockVR";

        private const string Package = "jira-bridge";

        private const string AssemblyRef = "AssemblyReference";

        public static void CreateAssemblyReference(Action<bool> complete)
        {
            if (DirectoryPathFinder.FindDirectoryPathInProjectByPattern(Package, out var packageFolder) && DirectoryPathFinder.FindDirectoryPathInProject(Target, out var targetFolder))
            {
#if UNITY_EDITOR
                Debug.Log($"SOURCE -> {packageFolder}, DESTINATION -> {targetFolder}");
#endif
                try
                {
                    var source = Path.Combine(packageFolder, "Samples~", AssemblyRef);

                    var destination = Path.Combine(targetFolder, AssemblyRef);

                    System.IO.File.Copy(source, Path.ChangeExtension(destination, File.Extension.asmref.Get()), true);

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
using UnityEngine;
using UnityEditor;

namespace Jira.Editor.Utility
{
    public static class ScriptingDefineUtility
    {
        public static void Set(string symbol, BuildTargetGroup target, bool define = true, bool log = false)
        {
            if (define)
            {
                Add(symbol, target, log);
            }
            else
            {
                Remove(symbol, target, log);
            }
        }

        public static void Add(string symbol, BuildTargetGroup target, bool log = false)
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

            if (definesString.Contains(symbol))
            {
                return;
            }

            var symbols = definesString.Split(';');

            ArrayUtility.Add(ref symbols, symbol);

            definesString = string.Join(";", symbols);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, definesString);

            if (log)
            {
                Debug.Log($"Added \"{symbol}\" from {EditorUserBuildSettings.selectedBuildTargetGroup} Scripting define in Player Settings");
            }
        }

        public static void Remove(string symbol, BuildTargetGroup target, bool log = false)
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

            if (!definesString.Contains(symbol))
            {
                return;
            }

            var symbols = definesString.Split(';');

            ArrayUtility.Remove(ref symbols, symbol);

            definesString = string.Join(";", symbols);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, definesString);

            if (log)
            {
                Debug.Log($"Removed \"{symbol}\" from {EditorUserBuildSettings.selectedBuildTargetGroup} Scripting define in Player Settings");
            }
        }
    }
}
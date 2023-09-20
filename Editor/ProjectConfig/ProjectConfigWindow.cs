using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Jira.Editor.ProjectConfig
{
    using Utility;
    using Runtime;
    using Jira.Runtime.Utility;
    using File = Jira.Runtime.Utility.File;

    public class ProjectConfigWindow : EditorWindow
    {
        private static bool _definedSymbol, _defineDebugging, _defineRockVR, _jiraSettings, _projectSettings;

        private static (string domain, string token, string user) _auth;

        private static (string issueType, string key) _project;

        [MenuItem("Jira/Configs")]
        private static void ShowWindow()
        {
            var window = GetWindow<ProjectConfigWindow>(false, "Jira Bridge", true);

            window.Show();
        }

        private void OnEnable()
        {
            GetEditorPrefs();

            if (_definedSymbol)
            {
                if (!Find(out _))
                {
                    new GameObject { name = "~JiraBridgeObject" }.AddComponent<OnGUIWindow>();

                    JiraClientSettings.Set(_auth.domain, _auth.user, _auth.token, _project.issueType, _project.key);
                }
            }
            else
            {
                if (Find(out var qa))
                {
                    DestroyImmediate(qa);
                }
            }
        }

        private void OnGUI()
        {
            SetEditorPrefs();

            _jiraSettings = Foldout(_jiraSettings, "Jira");

            if (_jiraSettings)
            {
                Label("Domain", EditorStyles.largeLabel);

                _auth.domain = TextField("", Validate(_auth.domain) ? _auth.domain.Contains(Constants.Https) ? _auth.domain : $"{Constants.Https}{_auth.domain}" : "");

                Label("User", EditorStyles.largeLabel);

                _auth.user = TextField("", _auth.user);

                Label("API Token", EditorStyles.largeLabel);

                var textFieldStyle = new GUIStyle(EditorStyles.textField)
                {
                    wordWrap = true
                };

                _auth.token = TextArea(_auth.token, textFieldStyle, GUILayout.Height(100));
            }

            _projectSettings = Foldout(_projectSettings, "Project");

            if (_projectSettings)
            {
                Label("Key", EditorStyles.largeLabel);

                _project.key = TextField("", _project.key.ToUpper());

                Label("Issue Type", EditorStyles.largeLabel);

                _project.issueType = TextField("", _project.issueType);
            }

            GUILayout.Space(10);

            if (Button(_definedSymbol ? "Disable Tool" : "Enable Tool", ButtonStyle(_definedSymbol ? Color.red : Color.white)))
            {
                _definedSymbol = !_definedSymbol;

                _defineDebugging = _definedSymbol;
            }

            GUILayout.Space(10);

            if (_definedSymbol && Button(_defineDebugging ? "Disable Debugging" : "Enable Debugging", ButtonStyle(_defineDebugging ? Color.red : Color.white)))
            {
                _defineDebugging = !_defineDebugging;
            }

            GUILayout.Space(10);

            if (Button(_defineRockVR ? "Undefined RockVR" : "Define RockVR", ButtonStyle(_defineRockVR ? Color.yellow : Color.white)))
            {
                _defineRockVR = !_defineRockVR;

                if (_defineRockVR && DirectoryPathFinder.FindDirectoryPathInProject(new[] { "Jira Bridge", "RockVR" }, out var paths))
                {
                    try
                    {
                        System.IO.File.Copy(Path.Combine(paths[0], "Runtime", "Samples", $"AssemblyReferences.{File.Extension.asmref.Get()}"), paths[1], true);

                        ScriptingDefineUtility.Set(Constants.RockVR, EditorUserBuildSettings.selectedBuildTargetGroup, _defineRockVR);
                    }
                    catch (Exception e)
                    {
                        _defineRockVR = false;
#if JIRA_DEBUGGING
                        Debug.LogError(e);
#else
                        _ = e;
#endif
                    }
                }
            }

            ScriptingDefineUtility.Set(Constants.ModuleDefinition, EditorUserBuildSettings.selectedBuildTargetGroup, _definedSymbol);

            ScriptingDefineUtility.Set(Constants.ModuleDebugging, EditorUserBuildSettings.selectedBuildTargetGroup, _defineDebugging);
        }

        private static void GetEditorPrefs()
        {
            _defineDebugging = EditorPrefs.GetBool(Constants.ModuleDebugging);

            _definedSymbol = EditorPrefs.GetBool(Constants.ModuleDefinition);

            _projectSettings = EditorPrefs.GetBool(Constants.Project);

            _jiraSettings = EditorPrefs.GetBool(Constants.Jira);

            _auth.domain = EditorPrefs.GetString(Constants.Domain);

            _auth.token = EditorPrefs.GetString(Constants.Token);

            _auth.user = EditorPrefs.GetString(Constants.User);

            _project.issueType = EditorPrefs.GetString(Constants.IssueType);

            _project.key = EditorPrefs.GetString(Constants.Key);

            _defineRockVR = EditorPrefs.GetBool(Constants.RockVR);
        }

        private static void SetEditorPrefs()
        {
            EditorPrefs.SetBool(Constants.ModuleDebugging, _defineDebugging);

            EditorPrefs.SetBool(Constants.ModuleDefinition, _definedSymbol);

            EditorPrefs.SetBool(Constants.Project, _projectSettings);

            EditorPrefs.SetBool(Constants.Jira, _jiraSettings);

            EditorPrefs.SetString(Constants.Domain, _auth.domain);

            EditorPrefs.SetString(Constants.Token, _auth.token);

            EditorPrefs.SetString(Constants.User, _auth.user);

            EditorPrefs.SetString(Constants.Key, _project.key);

            EditorPrefs.SetString(Constants.IssueType, _project.issueType);

            EditorPrefs.SetBool(Constants.RockVR, _defineRockVR);
        }

        private static GUIStyle ButtonStyle(Color color)
        {
            var style = new GUIStyle(EditorStyles.toolbarButton)
            {
                normal = { textColor = !_definedSymbol ? Color.white : color },
                fixedHeight = 50,
                fontSize = 20
            };

            style.fontSize = Mathf.Clamp(Screen.width / 16, 10, 25);

            return style;
        }

        #region HELPERS

        private static bool Find(out GameObject go)
        {
            go = GameObject.Find("~JiraBridgeObject");

            return go;
        }

        private static bool Validate(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        private static string TextField(string label, string text, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TextField(label, text, options);
        }

        private static void Label(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, style, options);
        }

        private static bool Foldout(bool foldout, string content)
        {
            return EditorGUILayout.Foldout(foldout, content);
        }

        private static bool Button(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayout.Button(text, style, options);
        }

        private static string TextArea(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TextArea(text, style, options);
        }

        #endregion
    }
}
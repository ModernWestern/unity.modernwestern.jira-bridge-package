using UnityEditor;
using UnityEngine;

namespace Jira.Editor.ProjectConfig
{
    using Utility;
    using Runtime;

    public class ProjectConfigWindow : EditorWindow
    {
        private static bool _definedJira, _defineJiraDebugging, _defineRockVR, _jiraSettingsFoldout, _jiraProjectSettingsFoldout;

        private static (string domain, string token, string user) _jiraAuth;

        private static (string issueType, string key) _jiraProject;

        [MenuItem("Jira/Configs")]
        private static void ShowWindow()
        {
            var window = GetWindow<ProjectConfigWindow>(false, "Jira Bridge", true);

            window.Show();
        }

        private void OnEnable()
        {
            GetEditorPrefs();

            if (_definedJira)
            {
                if (Find(out _))
                {
                    return;
                }

                // GitIgnore.ExcludeLogs();

                new GameObject { name = Constants.JiraObject }.AddComponent<OnGUIWindow>();

                JiraClientSettings.Set(_jiraAuth.domain, _jiraAuth.user, _jiraAuth.token.Clean(), _jiraProject.issueType, _jiraProject.key);
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

            _jiraSettingsFoldout = Foldout(_jiraSettingsFoldout, "Jira");

            if (_jiraSettingsFoldout)
            {
                Label("Domain", EditorStyles.largeLabel);

                var domain = Validate(_jiraAuth.domain) ? _jiraAuth.domain.Contains(Constants.Https) ? _jiraAuth.domain : $"{Constants.Https}{_jiraAuth.domain}" : string.Empty;

                _jiraAuth.domain = TextField(string.Empty, domain);

                Label("User", EditorStyles.largeLabel);

                _jiraAuth.user = TextField(string.Empty, _jiraAuth.user);

                Label("API Token", EditorStyles.largeLabel);

                var textFieldStyle = new GUIStyle(EditorStyles.textField)
                {
                    wordWrap = true
                };

                _jiraAuth.token = TextArea(_jiraAuth.token, textFieldStyle, GUILayout.Height(100));
            }

            _jiraProjectSettingsFoldout = Foldout(_jiraProjectSettingsFoldout, "Project");

            if (_jiraProjectSettingsFoldout)
            {
                Label("Key", EditorStyles.largeLabel);

                _jiraProject.key = TextField(string.Empty, _jiraProject.key.ToUpper());

                Label("Issue Type", EditorStyles.largeLabel);

                _jiraProject.issueType = TextField(string.Empty, _jiraProject.issueType);
            }

            GUILayout.Space(10);

            if (GUILayout.Button(JiraClientSettings.Exists ? "Update JSON" : "Create JSON"))
            {
                JiraClientSettings.Set(_jiraAuth.domain, _jiraAuth.user, _jiraAuth.token.Clean(), _jiraProject.issueType, _jiraProject.key);
            }

            GUILayout.Space(10);

            if (Button(_definedJira ? "Disable Tool" : "Enable Tool", ButtonStyle(_definedJira, Color.red)))
            {
                _definedJira = !_definedJira;

                _defineJiraDebugging = _definedJira;
            }

            GUILayout.Space(10);

            if (_definedJira && Button(_defineJiraDebugging ? "Disable Debugging" : "Enable Debugging", ButtonStyle(_defineJiraDebugging, Color.red)))
            {
                _defineJiraDebugging = !_defineJiraDebugging;
            }

            GUILayout.Space(10);

            RockVR();

            ScriptingDefineUtility.Set(Constants.Jira, EditorUserBuildSettings.selectedBuildTargetGroup, _definedJira);

            ScriptingDefineUtility.Set(Constants.JiraDebugging, EditorUserBuildSettings.selectedBuildTargetGroup, _defineJiraDebugging);
        }

        private static void RockVR()
        {
            if (Button(_defineRockVR ? "Undefined RockVR" : "Define RockVR", ButtonStyle(_defineRockVR, Color.yellow)))
            {
                _defineRockVR = !_defineRockVR;

                if (_defineRockVR)
                {
                    RockVRUtility.CreateAssemblyReference(done =>
                    {
                        if (done)
                        {
                            RockVRUtility.ModifyRockUtilsFile();
                            
                            ScriptingDefineUtility.Add(Constants.RockVR, EditorUserBuildSettings.selectedBuildTargetGroup);
                        }
                        else
                        {
                            _defineRockVR = false;
                        }
                    });
                }
                else
                {
                    ScriptingDefineUtility.Remove(Constants.RockVR, EditorUserBuildSettings.selectedBuildTargetGroup);
                }
            }
        }

        private static void GetEditorPrefs()
        {
            _jiraAuth.domain = Prefs.jiraAuthDomain;
            _jiraSettingsFoldout = Prefs.jiraSettingsFoldout;
            _jiraProject.key = Prefs.jiraProjectKey;
            _jiraProjectSettingsFoldout = Prefs.jiraProjectSettingsFoldout;
            _jiraAuth.token = Prefs.jiraAuthToken;
            _jiraAuth.user = Prefs.jiraAuthUser;
            _jiraProject.issueType = Prefs.jiraProjectIssueType;
            _defineJiraDebugging = Prefs.jiraDebugging;
            _definedJira = Prefs.defineJira;
            _defineRockVR = Prefs.defineRockVR;
        }

        private static void SetEditorPrefs()
        {
            Prefs.jiraAuthDomain = _jiraAuth.domain;
            Prefs.jiraSettingsFoldout = _jiraSettingsFoldout;
            Prefs.jiraProjectKey = _jiraProject.key;
            Prefs.jiraProjectSettingsFoldout = _jiraProjectSettingsFoldout;
            Prefs.jiraAuthToken = _jiraAuth.token;
            Prefs.jiraAuthUser = _jiraAuth.user;
            Prefs.jiraProjectIssueType = _jiraProject.issueType;
            Prefs.jiraDebugging = _defineJiraDebugging;
            Prefs.defineJira = _definedJira;
            Prefs.defineRockVR = _defineRockVR;
        }

        private static GUIStyle ButtonStyle(bool status, Color color)
        {
            var style = new GUIStyle(EditorStyles.toolbarButton)
            {
                normal = { textColor = !status ? Color.white : color },
                fixedHeight = 40,
                fontSize = Mathf.Clamp(Screen.width / 20, 9, 18)
            };

            return style;
        }

        #region HELPERS

        private static bool Find(out GameObject jiraBridgeObject)
        {
            jiraBridgeObject = GameObject.Find(Constants.JiraObject);

            return jiraBridgeObject;
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
using UnityEditor;

namespace Jira.Editor.ProjectConfig
{
    public static class Prefs
    {
        public static bool defineJira
        {
            get => EditorPrefs.GetBool(Constants.Jira);
            set => EditorPrefs.SetBool(Constants.Jira, value);
        }

        public static bool jiraDebugging
        {
            get => EditorPrefs.GetBool(Constants.JiraDebugging);
            set => EditorPrefs.SetBool(Constants.JiraDebugging, value);
        }

        public static bool jiraProjectSettingsFoldout
        {
            get => EditorPrefs.GetBool(Constants.ProjectFoldout);
            set => EditorPrefs.SetBool(Constants.ProjectFoldout, value);
        }

        public static bool jiraSettingsFoldout
        {
            get => EditorPrefs.GetBool(Constants.JiraFoldout);
            set => EditorPrefs.SetBool(Constants.JiraFoldout, value);
        }

        public static bool defineRockVR
        {
            get => EditorPrefs.GetBool(Constants.RockVR);
            set => EditorPrefs.SetBool(Constants.RockVR, value);
        }

        public static string jiraAuthDomain
        {
            get => EditorPrefs.GetString(Constants.JiraAuthDomain);
            set => EditorPrefs.SetString(Constants.JiraAuthDomain, value);
        }

        public static string jiraAuthToken
        {
            get => EditorPrefs.GetString(Constants.JiraAuthToken);
            set => EditorPrefs.SetString(Constants.JiraAuthToken, value);
        }

        public static string jiraAuthUser
        {
            get => EditorPrefs.GetString(Constants.JiraAuthUser);
            set => EditorPrefs.SetString(Constants.JiraAuthUser, value);
        }

        public static string jiraProjectKey
        {
            get => EditorPrefs.GetString(Constants.JiraProjectKey);
            set => EditorPrefs.SetString(Constants.JiraProjectKey, value);
        }

        public static string jiraProjectIssueType
        {
            get => EditorPrefs.GetString(Constants.JiraProjectIssueType);
            set => EditorPrefs.SetString(Constants.JiraProjectIssueType, value);
        }
    }
}
using UnityEngine;

namespace Jira.Runtime
{
    public static class JiraIssueConverter
    {
        public static string GetJson(string projectKey, string summary, string description, string issueType)
        {
            var data = new JiraIssueData(projectKey, summary, description, issueType);

            return JsonUtility.ToJson(data);
        }

        public static JiraIssueResponseData GetData(string json)
        {
            return JsonUtility.FromJson<JiraIssueResponseData>(json);
        }
    }
}
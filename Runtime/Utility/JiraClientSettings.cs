using System.IO;
using UnityEngine;

namespace Jira.Runtime
{
    public static class JiraClientSettings
    {
        private static readonly DirectoryInfo StreamingAssetsDirectory;

        private static readonly string FilePath;

        static JiraClientSettings()
        {
            StreamingAssetsDirectory = new DirectoryInfo(Application.streamingAssetsPath);

            FilePath = Path.Combine(StreamingAssetsDirectory.FullName, "JiraSettings.js");
        }

        public static JiraClient Get()
        {
            if (!File.Exists(FilePath))
            {
#if JIRA_DEBUGGING
                Debug.Log("The file JiraSettings.js does not exist, is corrupted or is empty. Re-enable the QA Tool");
#endif
                return null;
            }

            var data = JsonUtility.FromJson<JiraClient>(File.ReadAllText(FilePath));

#if JIRA_DEBUGGING
            Debug.Log($"FILE GET -> {data}");
#endif
            return data;
        }

        public static void Set(string domain, string user, string token, string issueId, string projectKey)
        {
            if (!StreamingAssetsDirectory.Exists)
            {
                StreamingAssetsDirectory.Create();
            }

            var client = new JiraClient
            {
                domain = domain,
                user = user,
                token = token,
                issueid = issueId,
                projectkey = projectKey
            };

#if JIRA_DEBUGGING
            Debug.Log($"FILE SET -> {client}");
#endif
            if (!File.Exists(FilePath))
            {
                File.Create(FilePath);
            }

            File.WriteAllText(FilePath, JsonUtility.ToJson(client));
        }
    }
}
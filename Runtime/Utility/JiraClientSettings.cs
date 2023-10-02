using System;
using System.IO;
using UnityEngine;

namespace Jira.Runtime
{
    public static class JiraClientSettings
    {
        private static readonly string FilePath;
        
        private static readonly DirectoryInfo StreamingAssetsDirectory;

        public static bool Exists => File.Exists(FilePath);

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
            try
            {
                using (var fileStream = File.Create(FilePath))
                {
                    var data = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(client));

                    fileStream.Write(data, 0, data.Length);

                    fileStream.Close();
                }

#if JIRA_DEBUGGING
                Debug.Log("JIRA -> Settings successfully written to the file.");
#endif
            }
            catch (IOException e)
            {
#if JIRA_DEBUGGING
                Debug.LogError("JIRA -> IOException: " + e.Message);
#else
                _ = e;
#endif
            }
            catch (Exception e)
            {
#if JIRA_DEBUGGING
                Debug.LogError("JIRA -> An error occurred: " + e.Message);
#else
                _ = e;
#endif
            }
        }
    }
}
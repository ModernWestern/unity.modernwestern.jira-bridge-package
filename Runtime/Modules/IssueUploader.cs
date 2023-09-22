using System;
using System.Text;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

namespace Jira.Runtime
{
    public class IssueUploader
    {
        private readonly string _issueUrl, _attachmentUrl, _auth;

        public JiraClient ClientData { get; }

        public bool HasData { get; }

        /// Domain = The organization's domain for the Jira instance (e.g., https://yourorganization.atlassian.net)
        /// Token = The API Token generated for the reporter. You can generate an API Token for your account at https://id.atlassian.com/manage-profile/security/api-tokens
        /// User = The email associated with the reporter's Jira account.
        public IssueUploader()
        {
            if (JiraClientSettings.Get() is { } data)
            {
                HasData = true;

                ClientData = new JiraClient
                {
                    domain = data.domain,
                    user = data.user,
                    token = data.token,
                    projectkey = data.projectkey,
                    issueid = data.issueid
                };

                _issueUrl = $"{ClientData.domain}/rest/api/2/issue";

                _attachmentUrl = $"{_issueUrl}/{{0}}/attachments";

                _auth = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientData.user}:{ClientData.token}"))}";
#if JIRA_DEBUGGING
                Debug.Log($"AUTH -> {_auth}");
#endif
            }
        }

        public void Post(string json, MonoBehaviour mono)
        {
            mono.StartCoroutine(Issue(json, (Action)null));
        }

        public void Post(string json, Action complete, MonoBehaviour mono)
        {
            mono.StartCoroutine(Issue(json, complete));
        }

        public void Post(string json, string imagePath, MonoBehaviour mono)
        {
            mono.StartCoroutine(Issue(json, handler => mono.StartCoroutine(Attachment(handler.text, imagePath, null))));
        }

        public void Post(string json, string imagePath, Action complete, MonoBehaviour mono)
        {
            mono.StartCoroutine(Issue(json, handler => mono.StartCoroutine(Attachment(handler.text, imagePath, complete))));
        }

        private IEnumerator Issue(string json, Action complete)
        {
            return Issue(json, _ => complete?.Invoke());
        }

        private IEnumerator Issue(string json, Action<DownloadHandler> complete)
        {
            var request = new UnityWebRequest(_issueUrl);

            request.method = UnityWebRequest.kHttpVerbPOST;

            request.SetRequestHeader("Authorization", _auth);

            request.SetRequestHeader("Content-Type", "application/json");

            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));

            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
#if JIRA_DEBUGGING
                Debug.Log($"POST SUCCESSFUL -> {request.downloadHandler.text}");
#endif
                complete?.Invoke(request.downloadHandler);

                yield break;
            }
#if JIRA_DEBUGGING
            Debug.Log($"POST FAILED -> {request.error}");
#endif
            complete?.Invoke(request.downloadHandler);
        }

//         private IEnumerator Attachment(string issueResponse, string path, Action complete)
//         {
//             if (!File.Exists(path))
//             {
//                 complete?.Invoke();
//
//                 yield break;
//             }
//
//             // var screenshotData = System.IO.File.ReadAllBytes(path);
//
//             var screenshotData = new FileStream(path, FileMode.Open, FileAccess.Read);
//
//             var issueResponseData = JiraIssueConverter.GetData(issueResponse);
//
// #if JIRA_DEBUGGING
//             Debug.Log($"SCREENSHOT PATH -> {path}\nBytes Length [{screenshotData.Length}]");
//
//             Debug.Log(issueResponseData);
// #endif
//             var request = new UnityWebRequest(string.Format(_attachmentUrl, issueResponseData.key));
//
//             request.method = UnityWebRequest.kHttpVerbPOST;
//
//             request.SetRequestHeader("Authorization", _auth);
//
//             request.SetRequestHeader("X-Atlassian-Token", "no-check");
//
//             request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(screenshotData.ToString()));
//
//             request.downloadHandler = new DownloadHandlerBuffer();
//
//             yield return request.SendWebRequest();
//
//             if (request.result == UnityWebRequest.Result.Success)
//             {
// #if JIRA_DEBUGGING
//                 Debug.Log($"SCREENSHOT ATTACHED SUCCESSFULLY -> {request.downloadHandler.text}");
// #endif
//                 complete?.Invoke();
//
//                 yield break;
//             }
// #if JIRA_DEBUGGING
//             Debug.Log($"ERROR ATTACHING SCREENSHOT -> {request.error}");
// #endif
//             complete?.Invoke();
//         }
//     }

        private IEnumerator Attachment(string issueResponse, string path, Action complete)
        {
            if (!File.Exists(path))
            {
                complete?.Invoke();

                yield break;
            }

            WWWForm form = new WWWForm();
            FileStream fileStream = new FileStream(path, FileMode.Open);
            byte[] fileBytes = new byte[fileStream.Length];
            var read = fileStream.Read(fileBytes, 0, fileBytes.Length);
            fileStream.Close();

            form.AddBinaryData("file", fileBytes, Path.GetFileName(path));

            var issueResponseData = JiraIssueConverter.GetData(issueResponse);

            using (var www = UnityWebRequest.Post(string.Format(_attachmentUrl, issueResponseData.key), form))
            {
                www.SetRequestHeader("Authorization", _auth);
//
                www.SetRequestHeader("X-Atlassian-Token", "no-check");

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error sending file: " + www.error);
                }
                else
                {
                    Debug.Log("File sent successfully!");

                    complete?.Invoke();
                }
            }
        }
    }
}
using System;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Jira.Runtime
{
    using Jira;

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
#if QA_DEBUG
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
#if QA_DEBUG
                Debug.Log($"POST SUCCESSFUL -> {request.downloadHandler.text}");
#endif
                complete?.Invoke(request.downloadHandler);

                yield break;
            }
#if QA_DEBUG
            Debug.Log($"POST FAILED -> {request.error}");
#endif
            complete?.Invoke(request.downloadHandler);
        }

        private IEnumerator Attachment(string issueResponse, string path, Action complete)
        {
            if (!System.IO.File.Exists(path))
            {
                complete?.Invoke();

                yield break;
            }

            var screenshotData = System.IO.File.ReadAllBytes(path);

            var issueResponseData = JiraIssueConverter.GetData(issueResponse);

#if QA_DEBUG
            Debug.Log($"SCREENSHOT PATH -> {path}\nBytes Length [{screenshotData.Length}]");

            Debug.Log(issueResponseData);
#endif
            var request = new UnityWebRequest(string.Format(_attachmentUrl, issueResponseData.key));

            request.method = UnityWebRequest.kHttpVerbPOST;

            request.SetRequestHeader("Authorization", _auth);

            request.SetRequestHeader("Content-Type", "image/png");

            request.SetRequestHeader("Content-Type", " application/octet-stream");

            request.SetRequestHeader("Content-Disposition", @$" form-data; name=""file""; filename=""{path}""");

            // // var uploadHandler = new UploadHandlerRaw(screenshotData);
            // //
            // // uploadHandler.contentType = "image/png";
            // //
            // // request.uploadHandler = uploadHandler;

            request.uploadHandler = new UploadHandlerRaw(screenshotData);

            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
#if QA_DEBUG
                Debug.Log($"SCREENSHOT ATTACHED SUCCESSFULLY -> {request.downloadHandler.text}");
#endif
                complete?.Invoke();

                yield break;
            }
#if QA_DEBUG
            Debug.Log($"ERROR ATTACHING SCREENSHOT -> {request.error}");
#endif
            complete?.Invoke();
        }
    }
}
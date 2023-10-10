using System;
using Utility;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Jira.Runtime
{
    public class IssueUploader
    {
        private int _attachmentsCount;
        
        private readonly string _issueUrl, _attachmentUrl, _auth;
        
        public JiraClient ClientData { get; }

        public bool HasData { get; }

        /// Domain = The organization's domain for the Jira instance (e.g., https://yourorganization.atlassian.net)
        /// Token = The API Token generated for the reporter. You can generate an API Token for your account at https://id.atlassian.com/manage-profile/security/api-tokens
        /// User = The email associated with the reporter's Jira account.
        public IssueUploader()
        {
            if (JiraClientSettings.Get() is not { } data)
            {
#if JIRA_DEBUGGING
                Debug.Log("JIRA -> JSON Settings not found");
#endif
                return;
            }

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

        public void Post(string json, MonoBehaviour mono)
        {
            mono.StartCoroutine(Issue(json, (Action)null));
        }

        public void Post(string json, Action complete, MonoBehaviour mono)
        {
            mono.StartCoroutine(Issue(json, complete));
        }

        public void Post(string json, string attachment, MonoBehaviour mono)
        {
            mono.StartCoroutine(Issue(json, handler => mono.StartCoroutine(Attachment(handler.text, attachment, null))));
        }

        public void Post(string json, string attachment, Action complete, MonoBehaviour mono)
        {
            mono.StartCoroutine(Issue(json, handler => mono.StartCoroutine(Attachment(handler.text, attachment, complete))));
        }

        public void Post(MonoBehaviour mono, string json, params string[] attachments)
        {
            mono.StartCoroutine(Issue(json, handler => mono.StartCoroutine(Attachments(null, handler.text, attachments))));
        }

        public void Post(MonoBehaviour mono, string json, Action complete, params string[] attachments)
        {
            mono.StartCoroutine(Issue(json, handler => mono.StartCoroutine(Attachments(complete, handler.text, attachments))));
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

                request.Dispose();

                yield break;
            }
#if JIRA_DEBUGGING
            Debug.Log($"POST FAILED -> {request.error}");
#endif
            complete?.Invoke(request.downloadHandler);

            request.Dispose();
        }

        private IEnumerator Attachments(Action complete, string issueResponse, params string[] paths)
        {
            return paths.Select(path => Attachment(issueResponse, path, () =>
            {
                _attachmentsCount++;

                if (_attachmentsCount != paths.Length)
                {
                    return;
                }

                _attachmentsCount = 0;

                complete?.Invoke();
                
            })).GetEnumerator();
        }

        private IEnumerator Attachment(string issueResponse, string path, Action complete)
        {
            if (!File.Exists(path))
            {
#if JIRA_DEBUGGING
                Debug.Log($"FILE [{(string.IsNullOrEmpty(path) ? "null" : path)}] DOES NOT EXIST");
#endif
                complete?.Invoke();

                yield break;
            }

            var form = new WWWForm();

            var fileStream = new FileStream(path, FileMode.Open);

            var fileBytes = new byte[fileStream.Length];

            fileStream.ReadAndClose(fileBytes, 0, fileBytes.Length);

            form.AddBinaryData("file", fileBytes, Path.GetFileName(path));

            var issueResponseData = JiraIssueConverter.GetData(issueResponse);

            using var request = UnityWebRequest.Post(string.Format(_attachmentUrl, issueResponseData.key), form);

            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Authorization", _auth);

            request.SetRequestHeader("X-Atlassian-Token", "no-check");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
#if JIRA_DEBUGGING
                Debug.Log($"FILE [{path}] ATTACHED SUCCESSFULLY -> {request.downloadHandler.text}");
#endif
                complete?.Invoke();

                request.Dispose();

                yield break;
            }
#if JIRA_DEBUGGING
            Debug.Log($"ERROR ATTACHING [{path}] FILE -> {request.error}");
#endif
            complete?.Invoke();

            request.Dispose();
        }
    }
}
using System.IO;
using UnityEngine;

namespace Jira.Runtime
{
    using Utility;

    public class OnGUIWindow : MonoBehaviour
    {
        private static Rect _rBox, _rTextSummary, _rLabelSummary, _rTextDescription, _rLabelDescription, _rIssueButton, _rEnableButton, _rOpenIssueReporterButton;

        private static readonly Vector2 ButtonSize = new(100, 50);

        private static GUIStyle _sTextField, _sTextArea, _sButton;

        private static string _inputSummary, _inputDescription;

        private static DirectoryInfo _documents, _attachments;

        private static bool _moduleActive, _windowOpen;

        #region MODULES

        private ScreenDocument _screenDocument;

#if ROCK_VR
        private ScreenRecorder _screenRecorder;
#endif
        private ScreenCapturer _screenCapturer;

        private IssueUploader _issueUploader;

        #endregion

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void OnEnable()
        {
#if UNITY_EDITOR

            _documents = new DirectoryInfo(Path.Combine(Application.dataPath, "..", "Jira Issues Log", Application.productName));
#else
            _documents = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "..", "Jira Issues Log", Application.productName));
#endif
            _attachments = new DirectoryInfo(Path.Combine(_documents.FullName, "Attachments"));
#if ROCK_VR
            _screenRecorder = new ScreenRecorder(gameObject, _attachments.FullName);
#endif
            _screenCapturer = new ScreenCapturer(_attachments.FullName);

            _screenDocument = new ScreenDocument(_documents.FullName, _screenCapturer);

            _issueUploader = new IssueUploader();
        }

        private void OnGUI()
        {
            Style();

            if (!_moduleActive && GUI.Button(_rEnableButton, "Start Jira Bridge"))
            {
                _moduleActive = !_moduleActive;
#if ROCK_VR
                _screenRecorder.StartRecording();
#endif
            }

            if (!_moduleActive)
            {
                return;
            }

            if (GUI.Button(_rOpenIssueReporterButton, "Create Issue"))
            {
                if (!_attachments.Exists)
                {
                    _attachments.Create();
                }

                if (!_windowOpen)
                {
                    _screenCapturer.Save();
#if ROCK_VR
                    if (_screenRecorder.IsRecording())
                    {
                        _screenRecorder.StopRecording();
                    }
#endif
                }

                Delay.NextFrame(() => _windowOpen = !_windowOpen, this);
            }

            if (!_windowOpen)
            {
                _inputSummary = string.Empty;

                _inputDescription = string.Empty;

                return;
            }

            GUI.Box(_rBox, $"{Application.productName} Issue");

            GUI.Label(_rLabelSummary, "Summary");

            _inputSummary = GUI.TextField(_rTextSummary, _inputSummary, _sTextField);

            GUI.Label(_rLabelDescription, "Description");

            _inputDescription = GUI.TextArea(_rTextDescription, _inputDescription, _sTextArea);

            if (GUI.Button(_rIssueButton, "Upload Issue", _sButton))
            {
                if (!_issueUploader.HasData || (string.IsNullOrEmpty(_inputSummary) && string.IsNullOrEmpty(_inputDescription)))
                {
                    return;
                }

                var json = JiraIssueConverter.GetJson(_issueUploader.ClientData.projectkey, _inputSummary, _inputDescription, _issueUploader.ClientData.issueid);

                _issueUploader.Post(json, _screenCapturer.LastFile, () => _windowOpen = false, this);

                // _issueUploader.Post(json, () => _issueUpload = true, this); // Without Attachment

                if (!_attachments.Exists)
                {
                    _attachments.Create();
                }

                _screenDocument.Save(_inputSummary, _inputDescription, _issueUploader.ClientData.projectkey, _issueUploader.ClientData.issueid);
            }
        }

        private static void Style()
        {
            _rEnableButton.x = 10;
            _rEnableButton.y = 10;
            _rEnableButton.width = ButtonSize.x;
            _rEnableButton.height = ButtonSize.y;

            _rOpenIssueReporterButton.x = Screen.width - ButtonSize.x - 10;
            _rOpenIssueReporterButton.y = Screen.height - ButtonSize.y - 10;
            _rOpenIssueReporterButton.width = ButtonSize.x;
            _rOpenIssueReporterButton.height = ButtonSize.y;

            _rBox.width = Screen.width * 0.5f;
            _rBox.height = Screen.height * 0.75f;

            _rBox.x = (Screen.width - _rBox.width) / 2;
            _rBox.y = (Screen.height - _rBox.height) / 2;

            _rTextSummary.x = _rBox.x + 10;
            _rTextSummary.y = _rBox.y + 70;
            _rTextSummary.width = _rBox.width - 30;
            _rTextSummary.height = 30;

            _rLabelSummary.x = _rTextSummary.x;
            _rLabelSummary.y = _rTextSummary.y - 30;
            _rLabelSummary.width = _rTextSummary.width;
            _rLabelSummary.height = 20;

            _rTextDescription.x = _rBox.x + 10;
            _rTextDescription.y = _rTextSummary.y + _rTextSummary.height + 40;
            _rTextDescription.width = _rBox.width - 20;
            _rTextDescription.height = _rBox.height - _rTextSummary.height - 160;

            _rLabelDescription.x = _rTextDescription.x;
            _rLabelDescription.y = _rTextDescription.y - 30;
            _rLabelDescription.width = _rTextDescription.width;
            _rLabelDescription.height = 20;

            _rIssueButton.x = _rBox.x + 10;
            _rIssueButton.y = _rBox.y + _rBox.height - 40;
            _rIssueButton.width = _rBox.width - 20;
            _rIssueButton.height = 30;

            _sTextField = new GUIStyle(GUI.skin.textField)
            {
                fontSize = Mathf.RoundToInt(_rBox.width * 0.015f),
                alignment = TextAnchor.MiddleLeft
            };

            _sTextArea = new GUIStyle(GUI.skin.textArea)
            {
                fontSize = Mathf.RoundToInt(_rBox.width * 0.015f)
            };

            _sButton = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(_rBox.width * 0.015f)
            };
        }
    }
}
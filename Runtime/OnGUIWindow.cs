using System.IO;
using UnityEngine;

namespace Jira.Runtime
{
    using Utility;

    public class OnGUIWindow : MonoBehaviour
    {
        private static Rect _rBox, _rTextSummary, _rLabelSummary, _rTextDescription, _rLabelDescription, _rIssueButton, _rEnableButton, _rOpenIssueReporterButton;

        private static readonly Vector2 ButtonSize = new(150, 50);

        private static DirectoryInfo _documents, _attachments, _tmp;

        private static GUIStyle _sTextField, _sTextArea, _sButton;

        private static string _inputSummary, _inputDescription;

        private static bool _start, _window, _upload;

        #region MODULES

        private IssueUploader _issueUploader;

        private ScreenDocument _screenDocument;

        private ScreenCapturer _screenCapturer;

#if ROCK_VR
        private ScreenRecorder _screenRecorder;
#endif

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
            _documents = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "Jira Issues Log", Application.productName));
#endif
            _attachments = new DirectoryInfo(Path.Combine(_documents.FullName, "Attachments"));

#if ROCK_VR
            _tmp = new DirectoryInfo(Path.Combine(_documents.FullName, "Temp"));

            _screenRecorder = new ScreenRecorder(gameObject, _tmp.FullName);
#endif
            _screenCapturer = new ScreenCapturer(_attachments.FullName);

            _screenDocument = new ScreenDocument(_documents.FullName, _screenCapturer);

            _issueUploader = new IssueUploader();
        }

        private void OnGUI()
        {
            Style();

            if (!_start && GUI.Button(_rEnableButton, "Start Jira Bridge"))
            {
                _start = !_start;
#if ROCK_VR
                _screenRecorder.StartRecording();
#endif
            }

            if (!_start)
            {
                return;
            }

            if (GUI.Button(_rOpenIssueReporterButton, "Create Issue"))
            {
                if (!_attachments.Exists)
                {
                    _attachments.Create();
                }

                if (!_window)
                {
                    _screenCapturer.Save();
#if ROCK_VR
                    if (_screenRecorder.IsRecording())
                    {
                        _screenRecorder.StopRecording();
                    }
#endif
                }

                Delay.NextFrame(() => _window = !_window, this);
            }

            if (!_window)
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

            if (!_upload && GUI.Button(_rIssueButton, "Upload Issue", _sButton))
            {
                if (!_issueUploader.HasData || (string.IsNullOrEmpty(_inputSummary) && string.IsNullOrEmpty(_inputDescription)))
                {
                    return;
                }

                var json = JiraIssueConverter.GetJson(_issueUploader.ClientData.projectkey, _inputSummary, _inputDescription, _issueUploader.ClientData.issueid);
#if ROCK_VR
                _issueUploader.Post(this, json, () =>
                {
                    _window = false;

                    _upload = false;

                    if (System.IO.File.Exists(_screenRecorder.FilePath))
                    {
                        System.IO.File.Delete(_screenRecorder.FilePath);
                    }

                    _screenRecorder.StartRecording();
                }, _screenRecorder.FilePath, _screenCapturer.FilePath); // Screenshot & Video
#else
                _issueUploader.Post(json, _screenCapturer.FilePath, () =>
                {
                    _window = false;

                    _upload = false;

                }, this); // Screenshot Only
#endif
                if (!_attachments.Exists)
                {
                    _attachments.Create();
                }

                _screenDocument.Save(_inputSummary, _inputDescription, _issueUploader.ClientData.projectkey, _issueUploader.ClientData.issueid);

                _upload = true;
            }
        }

        private void OnDestroy()
        {
            if (_screenRecorder.IsRecording())
            {
                const string projectCrash = "[project crash]";

                _screenCapturer.Save("CRASH");

                _screenDocument.Save(projectCrash, projectCrash, projectCrash, projectCrash);

                foreach (var file in _tmp.GetFiles())
                {
                    file.Delete();
                }
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
                fontSize = Mathf.RoundToInt(_rBox.width * 0.02f),
                alignment = TextAnchor.MiddleLeft
            };

            _sTextArea = new GUIStyle(GUI.skin.textArea)
            {
                fontSize = Mathf.RoundToInt(_rBox.width * 0.02f)
            };

            _sButton = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(_rBox.width * 0.02f)
            };
        }
    }
}
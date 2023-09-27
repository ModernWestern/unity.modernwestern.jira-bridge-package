#if ROCK_VR

using System;
using UnityEngine;
using RockVR.Video;

namespace Jira.Runtime
{
    public class ScreenRecorder
    {
        /// <summary>
        /// Returns the path of the file after Muxing.
        /// </summary>
        public event Action<string> OnMuxingComplete;

        public string FilePath => PathConfig.lastVideoFile;

        private readonly VideoCapture _videoCapture;

        private readonly VideoCaptureCtrl _videoCaptureController;

        public void StartRecording() => _videoCaptureController.StartCapture();

        public void StopRecording() => _videoCaptureController.StopCapture();

        public bool IsRecording() => _videoCaptureController.status == VideoCaptureCtrlBase.StatusType.STARTED;

        public ScreenRecorder(GameObject jiraBridgeObject, string path)
        {
            PathConfig.SaveFolder = $"{path}/";

            _videoCaptureController = jiraBridgeObject.AddComponent<VideoCaptureCtrl>();

#if JIRA_DEBUGGING
            _videoCaptureController.debug = true;
#else
            _videoCaptureController.debug = false;
#endif
            _videoCaptureController.startOnAwake = false;

            _videoCaptureController.captureTime = 10;

            _videoCapture = jiraBridgeObject.AddComponent<VideoCapture>();

            var recordingCamera = _videoCapture.GetComponent<Camera>();

            recordingCamera.CopyFrom(Camera.main);

            recordingCamera.targetDisplay = 1;

            _videoCapture.customPathFolder = path;

            _videoCapture.customPath = true;

            _videoCapture.isDedicated = true;

            _videoCapture.mode = VideoCaptureBase.ModeType.LOCAL;

            _videoCapture.frameSize = VideoCaptureBase.FrameSizeType._1920x1080;

            _videoCapture.encodeQuality = VideoCaptureBase.EncodeQualityType.Medium;

            _videoCapture._targetFramerate = VideoCaptureBase.TargetFramerateType._30;

            _videoCapture._antiAliasing = VideoCaptureBase.AntiAliasingType._2;

            _videoCaptureController.videoCaptures = new VideoCaptureBase[] { _videoCapture };

            _videoCaptureController.eventDelegate.OnComplete += () => OnMuxingComplete?.Invoke(PathConfig.lastVideoFile);

            if (Camera.main != null)
            {
                _videoCaptureController.audioCapture = Camera.main.gameObject.AddComponent<AudioCapture>();
            }
        }
    }
}

#endif
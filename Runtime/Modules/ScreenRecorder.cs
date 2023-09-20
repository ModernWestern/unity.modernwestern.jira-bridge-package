using UnityEngine;

#if ROCK_VR

using RockVR.Video;

namespace QA.Runtime
{
    public class ScreenRecorder
    {
        private readonly VideoCaptureCtrl _videoCaptureController;

        public void StartRecording() => _videoCaptureController.StartCapture();

        public void StopRecording() => _videoCaptureController.StopCapture();

        public bool IsRecording() => _videoCaptureController.status == VideoCaptureCtrlBase.StatusType.STARTED;

        public ScreenRecorder(GameObject qaObject, string path)
        {
            PathConfig.SaveFolder = path + "/";

            _videoCaptureController = qaObject.AddComponent<VideoCaptureCtrl>();

#if JIRA_DEBUGGING
            _videoCaptureController.debug = true;
#else
            _videoCaptureController.debug = false;
#endif
            _videoCaptureController.startOnAwake = false;

            _videoCaptureController.captureTime = 10;

            var videoCapture = qaObject.AddComponent<VideoCapture>();

            var recordingCamera = videoCapture.GetComponent<Camera>();

            recordingCamera.CopyFrom(Camera.main);

            recordingCamera.targetDisplay = 1;

            videoCapture.customPathFolder = path;

            videoCapture.customPath = true;

            videoCapture.isDedicated = true;

            videoCapture.frameSize = VideoCaptureBase.FrameSizeType._1920x1080;

            videoCapture.encodeQuality = VideoCaptureBase.EncodeQualityType.Medium;

            videoCapture._targetFramerate = VideoCaptureBase.TargetFramerateType._30;

            videoCapture._antiAliasing = VideoCaptureBase.AntiAliasingType._2;

            _videoCaptureController.videoCaptures = new VideoCaptureBase[] { videoCapture };

            _videoCaptureController.audioCapture = Camera.main.gameObject.AddComponent<AudioCapture>();
        }
    }
}

#endif
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Jira.Runtime
{
    [RequireComponent(typeof(EventSystem))]
    public class InputLogger
    {
        public string FilePath { get; }

        private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode));

        private bool _isRunning, _isKeyPressed;

        private MonoBehaviour _mono;

#if ROCK_VR
        private float _capturingTime;
#endif
        private float _keyTimer;

        public InputLogger(string path, MonoBehaviour mono)
        {
            _mono = mono;

            AssetDatabase.ImportAsset(path);

            FilePath = Path.Combine(path, "InputLog.txt");
        }

        public void Start() => _mono.StartCoroutine(LogRoutine());

        public void Stop() => _isRunning = false;

        private IEnumerator LogRoutine()
        {
            _isRunning = true;

            using var writer = new StreamWriter(FilePath);
#if ROCK_VR
            writer.Write("The gameplay started at [sync with video]: ");

            writer.WriteLine(_capturingTime);
#else
            writer.Write("The gameplay started at: ");

            writer.WriteLine(DateTime.Now);
#endif
            while (_isRunning)
            {
#if ROCK_VR
                _capturingTime += Time.deltaTime;
#endif
                if (Input.anyKeyDown)
                {
                    CheckForKeyCode(writer);
                }

                if (Input.anyKey)
                {
                    BeginKeyTimer();
                }
                else if (_keyTimer != 0)
                {
#if ROCK_VR
                    writer.WriteLine("Video time: " + _capturingTime.ToString("N2"));
#endif
                    writer.WriteLine("Key time: " + _keyTimer.ToString("N2"));

                    _keyTimer = 0;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    CheckForObjectName(writer);
                }

                yield return null;
            }

            writer.WriteLine(" ");
#if ROCK_VR
            writer.Write("The gameplay ended at [sync with video]: ");

            writer.WriteLine(_capturingTime);
#else
            writer.Write("The gameplay ended at: ");

            writer.WriteLine(DateTime.Now);
#endif
            _capturingTime = 0;

            writer.Close();
        }

        private void BeginKeyTimer() => _keyTimer += Time.unscaledDeltaTime;

        private void CheckForKeyCode(TextWriter writer)
        {
            foreach (KeyCode keyCode in keyCodes)
            {
                if (!Input.GetKey(keyCode))
                {
                    continue;
                }

                writer.WriteLine(" ");

                writer.WriteLine(DateTime.Now);

                writer.WriteLine("Key: " + keyCode);

                break;
            }
        }

        private void CheckForObjectName(TextWriter writer)
        {
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var raycastResults = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointer, raycastResults);

            if (raycastResults.Count <= 0)
            {
                return;
            }

            var gameObjectName = raycastResults[0].gameObject.name;

            var gameObjectType = raycastResults[0].gameObject.GetType();

            writer.WriteLine("Clicked GameObject: " + gameObjectName);

            writer.WriteLine("Type: " + gameObjectType);
        }
    }
}
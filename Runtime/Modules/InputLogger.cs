using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Jira.Runtime
{
    public class InputLogger
    {
        public string FilePath { get; }

        private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode));

        private bool _isRunning, _isKeyPressed;

        private MonoBehaviour _mono;

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

            writer.Write("The gameplay started at: ");

            writer.WriteLine(DateTime.Now);

            while (_isRunning)
            {
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
                    writer.WriteLine("Time: " + _keyTimer.ToString("N2"));

                    _keyTimer = 0;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    CheckForObjectName(writer);
                }

                yield return null;
            }

            writer.WriteLine(" ");

            writer.Write("The gameplay ended at: ");

            writer.WriteLine(DateTime.Now);

            writer.Close();
        }

        private void BeginKeyTimer() => _keyTimer += Time.unscaledDeltaTime;

        private void CheckForKeyCode(StreamWriter writer)
        {
            foreach (KeyCode keyCode in keyCodes)
            {
                if (Input.GetKey(keyCode))
                {
                    writer.WriteLine(" ");

                    writer.WriteLine(DateTime.Now);

                    writer.WriteLine("Key: " + keyCode);

                    break;
                }
            }
        }

        private void CheckForObjectName(StreamWriter writer)
        {
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var raycastResults = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointer, raycastResults);

            if (raycastResults.Count > 0)
            {
                var gameObjectName = raycastResults[0].gameObject.name;

                var gameObjectType = raycastResults[0].gameObject.GetType();

                writer.WriteLine("Clicked gameobject: " + gameObjectName);

                writer.WriteLine("Type: " + gameObjectType);
            }
        }
    }
}
using System;
using UnityEngine;
using System.Collections;

namespace Jira.Runtime.Utility
{
    public static class Delay
    {
        public static void NextFrame(Action callback, MonoBehaviour mono)
        {
            mono.StartCoroutine(NextFrame(callback));
        }

        private static IEnumerator NextFrame(Action callback)
        {
            yield return null;

            callback?.Invoke();
        }
    }
}
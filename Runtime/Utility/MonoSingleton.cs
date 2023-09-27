using UnityEngine;

namespace Utility
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static readonly object _lock = new object();

        private static T _instance;

        public static T instance
        {
            get
            {
                if (applicationIsQuitting)
                {
#if JIRA_DEBUGGING
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
#endif
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
#if JIRA_DEBUGGING
                            Debug.LogError("[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
#endif
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            var singleton = new GameObject();

                            _instance = singleton.AddComponent<T>();

                            singleton.name = typeof(T).Name;

                            DontDestroyOnLoad(singleton);
#if JIRA_DEBUGGING
                            Debug.Log($"[Singleton] An instance of {typeof(T)} is needed in the scene, so 'singleton' was created with DontDestroyOnLoad.");
#endif
                        }
#if JIRA_DEBUGGING
                        else
                        {
                            Debug.Log($"[Singleton] Using instance already created: {_instance.gameObject.name}");
                        }
#endif
                    }

                    return _instance;
                }
            }
        }

        /// <summary>
        /// If this script is placed on game objec in the scene, make sure to
        /// keep only instance.
        /// </summary>
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private static bool applicationIsQuitting = false;

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        /// it will create a buggy ghost object that will stay on the Editor scene
        /// even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        private void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }
    }
}
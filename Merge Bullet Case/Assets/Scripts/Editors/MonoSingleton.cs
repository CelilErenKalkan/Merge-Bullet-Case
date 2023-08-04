using UnityEngine;

namespace Editors
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static volatile T _instance;

        private bool isInitialized;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType(typeof(T)) as T;
                if (_instance != null && !_instance.isInitialized) Instance.Initialize();
                return _instance;
            }
        }

        protected virtual void Initialize()
        {
            isInitialized = true;
        }
    }
}
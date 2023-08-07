using UnityEngine;

namespace Editors
{
    /// <summary>
    /// A base class for creating a singleton MonoBehaviour instance.
    /// </summary>
    /// <typeparam name="T">The derived class type.</typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static volatile T _instance; // The singleton instance.

        private bool isInitialized; // Flag to track whether the instance has been initialized.

        /// <summary>
        /// Gets the singleton instance of the derived class.
        /// </summary>
        public static T Instance
        {
            get
            {
                // If the instance is already assigned, return it.
                if (_instance != null)
                    return _instance;

                // Otherwise, find the instance in the scene.
                _instance = FindObjectOfType(typeof(T)) as T;

                // If an instance was found and it hasn't been initialized, call Initialize().
                if (_instance != null && !_instance.isInitialized)
                    Instance.Initialize();

                return _instance;
            }
        }

        /// <summary>
        /// Initializes the singleton instance.
        /// Derived classes can override this method to perform initialization tasks.
        /// </summary>
        protected virtual void Initialize()
        {
            isInitialized = true;
        }
    }
}
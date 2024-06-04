using UnityEngine;

namespace NeoForge.Utilities
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _isInitialized;

        public static T Instance
        {
            get
            {
                if (!_isInitialized)
                {
                    _instance = FindObjectOfType<T>();
                    _isInitialized = true;
                }

                return _instance;
            }
        }
        
        protected virtual void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
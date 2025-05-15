using UnityEngine;
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T _instance;

    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (!_instance) {
            _instance = this as T;
        }
        else if (_instance != this) {
            Destroy(gameObject);
        }
    }
}
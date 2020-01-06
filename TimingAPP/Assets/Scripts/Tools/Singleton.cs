using UnityEngine;
using UnityEditor;

public class Singleton<T>
{
    private static T _instance;

    protected Singleton() {
        Initialize();
    }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)System.Activator.CreateInstance(typeof(T));
            }
            return _instance;
        }
    }

    public virtual void Initialize() { }
}
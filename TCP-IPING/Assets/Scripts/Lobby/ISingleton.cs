using UnityEngine;
using System;

public abstract class ISingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly Lazy<T> lazyInstance = new Lazy<T>(CreateSingleton);

    public static T Instance => lazyInstance.Value;

    private static T CreateSingleton()
    {
        T existingInstance = FindObjectOfType<T>();

        if (existingInstance != null)
        {
            return existingInstance;
        }
        
        GameObject singletonObject = new GameObject(typeof(T).Name);
        T newInstance = singletonObject.AddComponent<T>();
        DontDestroyOnLoad(singletonObject);
        return newInstance;
    }

    protected virtual void Awake()
    {
        if (lazyInstance.IsValueCreated && lazyInstance.Value != this)
        {
            Destroy(gameObject);
        }
    }
}

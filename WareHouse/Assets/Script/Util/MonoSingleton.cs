using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T instance;
    private static object _lock = new object();
    private static bool shuttingDown = false;

    public static T Instance
    {
        get
        {
            if (shuttingDown)
            {
                Debug.LogWarning($"MonoSingletonObject {typeof(T).ToString()} is already destoryed...");
                return null;
            }

            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;

                if (instance == null)
                {
                    var singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    singletonObject.name = $"{typeof(T).ToString()} (Singleton)";

                    DontDestroyOnLoad(singletonObject);
                }
            }
        
            return instance;
        }
    }

    private void OnApplicationQuit()
    {
        shuttingDown = false;
    }

    private void OnDestroy()
    {
        shuttingDown = true;
    }
}

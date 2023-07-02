using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T :MonoBehaviour
{
    private static T _instnace;
    
    public static T Instance
    {
        get
        {
            if(_instnace == null)
            {
                _instnace = FindObjectOfType<T>();

                if( _instnace == null)
                {
                    GameObject obj = new GameObject();
                    _instnace = obj.AddComponent<T>();
                    
;                    // DontDestroyOnLoad(obj);
                }
            }

            return _instnace;
        }
    }
}

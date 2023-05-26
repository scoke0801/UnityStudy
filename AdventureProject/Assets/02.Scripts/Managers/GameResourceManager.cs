using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameResourceManager
{
    Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, UnityEngine.Object>();

    public T Load<T>(string key) where T : UnityEngine.Object
    {
        if(_resources.TryGetValue(key, out UnityEngine.Object resources))
        {
            return resources as T;
        }
        return null;
    }

    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>(key);
        if(prefab)
        {
            return null;
        }

        GameObject go = UnityEngine.Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
    }

    public void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        if(_resources.TryGetValue(key, out UnityEngine.Object resource))
        {
            callback?.Invoke(resource as T);
            return;
        }

        var asyncOperation = Addressables.LoadAssetAsync<T>(key);
        asyncOperation.Completed += (operation) =>
        {
            _resources.Add(key, operation.Result);
            callback.Invoke(operation.Result);
        };
    }

    public void LoadAllAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
    {
        var asyncOperation = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        asyncOperation.Completed += (operation) =>
        {
            int loadCount = 0;
            int totalCount = operation.Result.Count;

            foreach (var result in operation.Result)
            {
                Addressables.LoadAssetAsync<T>(result.PrimaryKey).Completed += (innerOperation) =>
                {
                    if( !_resources.ContainsKey(result.PrimaryKey))
                    {
                        _resources.Add(result.PrimaryKey, innerOperation.Result);
                    }
                    ++loadCount;
                    callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                };
            }
        };
    }
    
    public void Init()
    {

    }
}

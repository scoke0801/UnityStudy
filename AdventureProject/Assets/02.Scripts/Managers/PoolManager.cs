using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class Pool
{
    // TODO. Addressable 사용하는 대상을 Pool로 다룰 떄의 처리.

    string _name;
    GameObject _prefab;
    IObjectPool<GameObject> _pool;

    Transform _root;
    Transform Root
    {
        get
        {
            if(_root == null)
            {
                GameObject go = new GameObject() { name = $"{_name}Root" };
                _root = go.transform;
            }

            return _root;
        }
    }

    public Pool(string name, GameObject prefab)
    {
        _name = name;
        _prefab = prefab;
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    public void Release(GameObject go)
    {
        _pool.Release(go);
    }

    public GameObject Get()
    {
        return _pool.Get();
    }

    #region ObjectPool CallbackFuncs
    GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(_prefab);
        go.transform.parent = Root;
        go.name = _prefab.name;
        return go;
    }
    void OnGet(GameObject go)
    {
        go.SetActive(true);
    }

    void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }
    void OnDestroy(GameObject go)
    {
        GameObject.Destroy(go);
    }
    #endregion

}
public class PoolManager
{
    Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    public GameObject Pop(string key, GameObject prefab)
    {
        if(_pools.ContainsKey(key) == false)
        {
            CreatePool(key, prefab);
        }
        return _pools[key].Get();
    }

    public bool Push(string key, GameObject go)
    {
        if(_pools.ContainsKey(key)== false)
        {
            return false;
        }

        _pools[key].Release(go);
        return true;
    }
    void CreatePool(string key, GameObject prefab)
    {
        Pool pool = new Pool(key, prefab);
        _pools.Add(key, pool);
    }
}

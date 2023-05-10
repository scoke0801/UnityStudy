using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _poolSize = 10;

    private Queue<GameObject> _inactiveObjects = new Queue<GameObject>();
    private List<GameObject> _activeObjects = new List<GameObject>();

    public void Init(GameObject prefab, int size)
    {
        _prefab = prefab;
        _poolSize = size;

        for (int i = 0; i < _poolSize; ++i)
        {
            GameObject instance = GameObject.Instantiate(_prefab);
            instance.SetActive(false);
            _inactiveObjects.Enqueue(instance);
        }
    }
    public GameObject GetObject()
    {
        GameObject spawnedObject;

        if (_inactiveObjects.Count > 0)
        {
            spawnedObject = _inactiveObjects.Dequeue();
        }
        else
        {
            spawnedObject = GameObject.Instantiate(_prefab);
            _activeObjects.Add(spawnedObject);
        }
        return spawnedObject;
    }

    public void ReturnObject(GameObject returnedObject)
    {
        if (_activeObjects.Remove(returnedObject))
        {
            returnedObject.SetActive(false);
            _inactiveObjects.Enqueue(returnedObject);
        }
    }
}

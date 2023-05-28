using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject _prefab;

    private string _key;

    public int _spanwCount;

    private void Awake()
    {
        _key = "Monster";
        StartCoroutine(nameof(SpawnRoutine));
    }

    IEnumerator SpawnRoutine()
    {
        int spawnCount = 0;
        while(spawnCount < _spanwCount)
        {
            GameObject instance =  Managers.Instance.PoolMgr.Pop(_key, _prefab);
            instance.transform.position = new Vector3(Random.Range(-15.0f, 15.0f), 0f, Random.Range(-15.0f, 15.0f));
            ++spawnCount;

            yield return new WaitForSeconds(0.5f);
        }
    }
}

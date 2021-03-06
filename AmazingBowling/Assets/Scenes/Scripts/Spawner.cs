using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private BoxCollider area;

    public GameObject[] propPrefabs;

    public int count = 100;

    private List<GameObject> props = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        area = GetComponent<BoxCollider>();

        for (int i = 0; i < count; ++i)
        {
            // 생성용 함수
            Spawn();
        }

        area.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Spawn()
    {
        int selection = Random.Range(0, propPrefabs.Length);

        GameObject selectedPrefab = propPrefabs[selection];

        Vector3 spawnPos = GetRandomPos();

        GameObject instance = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        
        props.Add(instance);
    }

    private Vector3 GetRandomPos()
    {
        Vector3 basePosition = transform.position;
        Vector3 size = area.size;

        float posX = basePosition.x + Random.Range(-size.x * 0.5f, size.x * 0.5f);
        float posY = basePosition.y + Random.Range(-size.y * 0.5f, size.y * 0.5f);
        float posZ = basePosition.z + Random.Range(-size.z * 0.5f, size.z * 0.5f);

        return new Vector3(posX, posY, posZ);
    }

    public void Reset()
    {
        for (int i = 0; i < props.Count; ++i)
        {
            props[i].transform.position = GetRandomPos();
            props[i].SetActive(true);
        }
    }

}

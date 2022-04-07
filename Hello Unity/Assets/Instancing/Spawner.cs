using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject target;

    public Transform spawnPosition;
    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(target);
        GameObject instance = Instantiate(target, spawnPosition.position, spawnPosition.rotation);

        Debug.Log(instance.name);
    } 
}

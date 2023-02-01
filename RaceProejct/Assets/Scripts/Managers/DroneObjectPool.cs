using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DroneObjectPool : MonoBehaviour
{
    public int maxPoolSize = 10;
    public int stackDefaultCapacity = 10;

    public IObjectPool<Drone> Pool
    {
        get
        {
            if(_pool == null)
            {
                _pool = new ObjectPool<Drone>(
                    CreatedPooledItem,
                    OnTakeFromPool,
                    OnReturendToPool,
                    OnDestroyPoolObject,
                    true,
                    stackDefaultCapacity,
                    maxPoolSize);
            }
            return _pool;
        }
    }

    private IObjectPool<Drone> _pool;

    private Drone CreatedPooledItem()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Drone drone = obj.AddComponent<Drone>();

        obj.name = "Drone";
        drone.Pool = Pool;

        return drone;
    }

    private void OnReturendToPool(Drone drone)
    {
        drone.gameObject.SetActive(false);
    }

    private void OnTakeFromPool(Drone drone)
    {
        drone.gameObject.SetActive(true);
    }

    private void OnDestroyPoolObject(Drone drone)
    {
        Destroy(drone.gameObject);
    }

    public void Spawn()
    {
        int amount = Random.Range(1, 10);

        for(int i = 0; i < amount; ++i)
        {
            Drone drone = Pool.Get();

            drone.transform.position = Random.insideUnitSphere * 10;
        }
    }
}

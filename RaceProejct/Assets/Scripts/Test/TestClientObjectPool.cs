using System.Collections;
using UnityEngine;

public class TestClientObjectPool : MonoBehaviour
{
    private DroneObjectPool _pool;

    // Use this for initialization
    void Start()
    {
        _pool = gameObject.AddComponent<DroneObjectPool>();
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Spawn Drone"))
        {
            _pool.Spawn();
        }
    }
}
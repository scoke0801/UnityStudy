using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance = null;

    public GameObject missilePrefab;

    int maxNum = 8;

    public List<GameObject> missilePools = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this.gameObject);
        MakeMissile();
    }

    public GameObject GetMissile()
    {
        for (int i = 0; i < missilePools.Count; ++i)
        {
            if(missilePools[i].activeSelf == false)
            {
                return missilePools[i];
            }
        }
        return null;
    }

    void MakeMissile()
    {
        GameObject ObjPools = new GameObject("ObjPools");

        for(int i =0; i < maxNum; ++i)
        {
            var poolObj = Instantiate<GameObject>(missilePrefab, ObjPools.transform);
            poolObj.SetActive(false);
            poolObj.name = "Missile_" + i.ToString("0");
            missilePools.Add(poolObj);
        }
    }
}

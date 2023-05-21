using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _instance = null;
    public static Managers Instance { get { return _instance; } }

    private static GameResourceManager _resourceManager = new GameResourceManager();
    private static PoolManager _poolManager = new PoolManager();

    public static GameResourceManager ResMgr {  get { return _resourceManager; } }
    public static PoolManager PoolMgr { get { return _poolManager; } }

    private void Awake()
    {
        Init();
    }
    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };

                _instance = go.AddComponent<Managers>();
                DontDestroyOnLoad(go);

                _resourceManager.Init();
                _poolManager.Init();
            }
        }
    }
}

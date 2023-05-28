using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static bool _init = false;
    static Managers _instance = null;
    public static Managers Instance
    {
        get
        {
            if (_init == false)
            {
                _init = true;
                GameObject go = GameObject.Find("@Managers");
                if (go == null)
                {
                    go = new GameObject { name = "@Managers" };

                    _instance = go.AddComponent<Managers>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private GameResourceManager _resourceManager = new GameResourceManager();
    private PoolManager _poolManager = new PoolManager();

    public GameResourceManager ResMgr {  get { return _instance?._resourceManager; } }
    public PoolManager PoolMgr { get { return _instance?._poolManager; } }

    private void Awake()
    {
        Init();
    }

    private static void Init()
    {
        if (_init == false) { return; }

        _instance?._resourceManager.Init();
    }
}

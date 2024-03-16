using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoSingleton<AppManager>
{
    private void Awake()
    {
        // 초기화를 위해 미리 호출.
        var temp = AppManager.Instance;
        DontDestroyOnLoad(temp);

        var audioManager = SoundManager.Instance; 
    }
}

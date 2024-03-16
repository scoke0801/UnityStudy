using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoSingleton<AppManager>
{
    private void Awake()
    {
        // �ʱ�ȭ�� ���� �̸� ȣ��.
        var temp = AppManager.Instance;
        DontDestroyOnLoad(temp);

        var audioManager = SoundManager.Instance; 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이펙트 프리팹과 경로와 타입등의 속성 데이터를 가지고 있게되며
/// 프리팹 사전로딩 기능을 갖고 있고 - 풀링을 위한 기능이기도 합니다.
/// 이펙트 인스턴스 기능을 갖고 있으며 - 풀링과 연계하여 사용하기도 합니다.
/// </summary>
public class EffectClip 
{
    public int realId = 0;

    public EffectType effectType = EffectType.NORMAL;

    public GameObject effectPrefab = null;
    public string effectName = string.Empty;
    public string effectPath = string.Empty;
    public string effectFullPath = string.Empty;

    public EffectClip() { }

    public void PreLoad()
    {
        effectFullPath = effectPath + effectName;
        if(effectFullPath != string.Empty && effectPrefab == null)
        {
            effectPrefab = ResourceManager.Load(effectPath) as GameObject;
        }
    }

    public void ReleaseEffect()
    {
        if(effectPrefab != null)
        {
            effectPrefab = null;
        }
    }

    public GameObject Instantiate(Vector3 pos)
    {
        if(effectPrefab == null)
        {
            PreLoad();
        }
        
        if(effectPrefab != null)
        {
            GameObject effect = GameObject.Instantiate(effectPrefab, pos, Quaternion.identity);
            return effect;
        }

        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����Ʈ �����հ� ��ο� Ÿ�Ե��� �Ӽ� �����͸� ������ �ְԵǸ�
/// ������ �����ε� ����� ���� �ְ� - Ǯ���� ���� ����̱⵵ �մϴ�.
/// ����Ʈ �ν��Ͻ� ����� ���� ������ - Ǯ���� �����Ͽ� ����ϱ⵵ �մϴ�.
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

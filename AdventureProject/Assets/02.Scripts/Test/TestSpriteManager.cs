using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpriteManager : MonoBehaviour
{
    private static TestSpriteManager _instance;

    public static TestSpriteManager Instante
    {
        get
        {
            return _instance;
        }
    }


    [SerializeField] private List<Sprite> _sprites;

    private void Awake()
    {
        if(_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    public Sprite GetRandomSprite()
    {
        if(_sprites == null) { return null; }
        int min = 0;
        int max = _sprites.Count;

        int idx = Random.Range(min, max);

        return _sprites[idx];
    }
}

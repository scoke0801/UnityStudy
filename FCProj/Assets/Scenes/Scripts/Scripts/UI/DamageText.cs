using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public float _delayTimeToDestroy = 1.0f;
    private void Start()
    {
        Destroy(gameObject, _delayTimeToDestroy);
    }
}

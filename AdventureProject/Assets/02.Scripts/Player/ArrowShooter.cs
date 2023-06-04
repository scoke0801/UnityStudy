using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShooter : MonoBehaviour
{
    [SerializeField] GameObject _prefab;
    [SerializeField] float _speed = 10.0f;

    private GameObject _arrow;
    private Vector3 _direction;

    private void Update()
    {
        if (!_arrow)
        {
            return;
        }

        _arrow.transform.position = _arrow.transform.position + _direction * _speed * Time.deltaTime;
    }

    public void Attack(Vector3 position, Vector3 dir)
    {
        _direction = dir.normalized;

        _arrow = GameObject.Instantiate(_prefab);
        _arrow.transform.position = position;

        GameObject.Destroy(_arrow, 5.0f );
    }
}

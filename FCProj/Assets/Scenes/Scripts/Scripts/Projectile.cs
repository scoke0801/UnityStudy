using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Variables


    public float _speed;
    public GameObject _muzzlePrefab;
    public GameObject _hitPrefab;

    public AudioClip _shotSFX;
    public AudioClip _hitSFX;

    private bool _collided;
    private Rigidbody _rigidbody;

    [HideInInspector]
    public AttackBehaviour _attackBehaviour;

    [HideInInspector]
    public GameObject _owner;

    [HideInInspector]
    public GameObject _target;

    private AudioSource _audioSource;
    #endregion

    private void Start()
    {
        if(_target != null)
        {
            Vector3 dest = _target.transform.position;
            dest.y += 1.5f; // 대상 위치에 따라서 다른 조정이 필요할수도...

            transform.LookAt(dest);
        }
        if (_owner)
        {
            Collider projectileCollider = GetComponent<Collider>();
            
            // 총알 발사자와 총알이 충돌하지 않도록 설정.
            Collider[] ownerColliders = _owner.GetComponentsInChildren<Collider>();

            foreach(Collider collider in ownerColliders)
            {
                Physics.IgnoreCollision(projectileCollider, collider);
            }
        }

        if (_muzzlePrefab)
        {
            GameObject muzzleVFX = Instantiate(_muzzlePrefab, transform.position, Quaternion.identity);

            // 바라보는 방향이 동일하게.
            muzzleVFX.transform.forward = gameObject.transform.forward;

            ParticleSystem particleSystem = muzzleVFX.GetComponent<ParticleSystem>();
            if (particleSystem)
            {
                Destroy(muzzleVFX, particleSystem.main.duration);
            }
            else
            {
                ParticleSystem childParticleSystem = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                if (childParticleSystem)
                {
                    Destroy(muzzleVFX, childParticleSystem.main.duration);
                }
            }

            _audioSource = GetComponent<AudioSource>();
            if(_shotSFX != null && _audioSource)
            {
                _audioSource.PlayOneShot(_shotSFX);
            }
        }
    }

    private void FixedUpdate()
    {
        if(_speed != 0 && _rigidbody != null)
        {
            _rigidbody.position += (transform.forward) * (_speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_collided)
        {
            return;
        }

        _collided = true;

        // 처리 중 다른 물체와 부딪혀 오동작 하는 것을 막기위한 설정.
        Collider projectilCollider = GetComponent<Collider>();
        projectilCollider.enabled = false;

        if (_hitSFX != null && _audioSource)
        {
            _audioSource.PlayOneShot(_hitSFX);
        }


        _speed = 0;

        // 물리 운동을 하지 않도록.
        _rigidbody.isKinematic = true;

        if (_hitPrefab)
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion contactRotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

            Vector3 contactPosition = contact.point;
            GameObject hitVFX = Instantiate(_hitPrefab, contactPosition, contactRotation);

            ParticleSystem particleSystem = hitVFX.GetComponent<ParticleSystem>();
            if (particleSystem)
            {
                Destroy(hitVFX, particleSystem.main.duration);
            }
            else
            {
                ParticleSystem childParticleSystem = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                if (childParticleSystem)
                {
                    Destroy(hitVFX, childParticleSystem.main.duration);
                }
            }
        }
    }
}

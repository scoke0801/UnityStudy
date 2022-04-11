using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public ParticleSystem explosionParticle;

    public AudioSource explosionAudio;

    public float maxDamage = 100.0f;

    public float explosionForce = 1000f;

    public float lifeTime = 10.0f;

    public float explosionRadius = 20.0f;

    public LayerMask whatIsProp;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // 인자 범위 안에 해당하는 가상의 구를 그려서 해당하는 오브젝트를 배열로 반환
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, whatIsProp);
        
        for (int i = 0; i < colliders.Length; ++i)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            Prop targetProp = colliders[i].GetComponent<Prop>();

            float damage = CalculateDamage(targetProp.transform.position);

            targetProp.TakeDamage(damage);
        }
        // 부모 해제
        // 아래에서 object를 삭제할 때, 사라지지 않도록
        explosionParticle.transform.parent = null; 
        explosionParticle.Play(); 
        explosionAudio.Play(); 

        Destroy(explosionParticle, explosionParticle.duration);
        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explositionToTarget = targetPosition - transform.position;

        // 거리
        float distance = explositionToTarget.magnitude;

        float edgeToCenterDistance = explosionRadius - distance;

        float percentage = edgeToCenterDistance / explosionRadius;

        float damage = maxDamage * percentage;
        
        damage = Mathf.Max(damage, 0);

        return damage;
    }

    private void OnDestroy()
    {
        GameManager.instance.OnBallDestroy();
    }
}

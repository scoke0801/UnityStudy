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
        // ���� ���� �ȿ� �ش��ϴ� ������ ���� �׷��� �ش��ϴ� ������Ʈ�� �迭�� ��ȯ
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, whatIsProp);
        
        for (int i = 0; i < colliders.Length; ++i)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            Prop targetProp = colliders[i].GetComponent<Prop>();

            float damage = CalculateDamage(targetProp.transform.position);

            targetProp.TakeDamage(damage);
        }
        // �θ� ����
        // �Ʒ����� object�� ������ ��, ������� �ʵ���
        explosionParticle.transform.parent = null; 
        explosionParticle.Play(); 
        explosionAudio.Play(); 

        Destroy(explosionParticle, explosionParticle.duration);
        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explositionToTarget = targetPosition - transform.position;

        // �Ÿ�
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

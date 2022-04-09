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
        // �θ� ����
        // �Ʒ����� object�� ������ ��, ������� �ʵ���
        explosionParticle.transform.parent = null; 
        explosionParticle.Play(); 
        explosionAudio.Play();

        Destroy(explosionParticle, explosionParticle.duration);
        Destroy(gameObject);
    }
}

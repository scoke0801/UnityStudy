using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public int score = 5;
    public ParticleSystem explosionPartice;

    public float hp = 10.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damaga)
    {
        hp -= damaga;
        if (hp <= 0)
        {
            // ���� ���� ������Ʈ�� ��������� ��������
            ParticleSystem instance = Instantiate(explosionPartice, transform.position, transform.rotation);
            instance.Play();

            AudioSource audioSource = instance.GetComponent<AudioSource>();
            audioSource.Play();

            GameManager.instance.AddScore(score);

            Destroy(instance.gameObject, instance.duration);

            // Prop�� �ı����� ��Ȱ��
            gameObject.SetActive(false);
        }
    }
}

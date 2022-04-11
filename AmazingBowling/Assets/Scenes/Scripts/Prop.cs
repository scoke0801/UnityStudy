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
            // 원본 게임 오브젝트를 집어넣으면 복사해줌
            ParticleSystem instance = Instantiate(explosionPartice, transform.position, transform.rotation);
            instance.Play();

            AudioSource audioSource = instance.GetComponent<AudioSource>();
            audioSource.Play();

            GameManager.instance.AddScore(score);

            Destroy(instance.gameObject, instance.duration);

            // Prop은 파괴보단 재활용
            gameObject.SetActive(false);
        }
    }
}

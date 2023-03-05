using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    public GameObject bullet;

    public Transform firePos;

    public AudioClip fireSfx;

    private new AudioSource audio;

    private MeshRenderer muzzleFlash;

    private RaycastHit raycastHit;

    private float RAY_DIST = 10.0f;

    private void Start()
    {
        audio = GetComponent<AudioSource>();

        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        muzzleFlash.enabled = false;
    }

    private void Update()
    {
        Debug.DrawRay(firePos.position, firePos.forward * RAY_DIST, Color.green);

        if(Input.GetMouseButtonDown(0))
        {
            Fire();

            if( Physics.Raycast(firePos.position,
                firePos.forward,
                out raycastHit,
                RAY_DIST,
                1 << 6))
            {
                Debug.Log($"Hit={raycastHit.transform.name}");
                raycastHit.transform.GetComponent<MonsterCtrl>()?.OnDamage(raycastHit.point, raycastHit.normal);
            }
        }
    }

    void Fire()
    {
        Instantiate(bullet, firePos.position, firePos.rotation);

        audio.PlayOneShot(fireSfx, 1.0f);

        StartCoroutine(ShowMuzzleFlash());
    }

    IEnumerator ShowMuzzleFlash()
    {
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        muzzleFlash.material.mainTextureOffset = offset;

        float angle = Random.Range(0, 360);
        muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, angle);

        float scale = Random.Range(0.5f, 1.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        muzzleFlash.enabled = true;

        yield return new WaitForSeconds(0.2f);

        muzzleFlash.enabled = false;
    }
}


using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Transform firePos;
    public GameObject bulletPrefab;
    private ParticleSystem muzzleFlash;

    private PhotonView photonView;
    private bool isMouseClick => Input.GetMouseButtonDown(0);

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        muzzleFlash = firePos.Find("MuzzleFlash").GetComponent<ParticleSystem>();
        
    }

    void Update()
    {
        if (photonView.IsMine && isMouseClick)
        {
            FireBullet(photonView.Owner.ActorNumber);
            // RPC로 원격지에 있는 함수를 호출.
            photonView.RPC("FireBullet", RpcTarget.Others, photonView.Owner.ActorNumber);
        }
    }

    [PunRPC]
    void FireBullet(int actorNum)
    {
        if (!muzzleFlash.isPlaying) muzzleFlash.Play();

        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);

        bullet.GetComponent<Bullet>().actorNumber = actorNum;
    }
}

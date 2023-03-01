using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Damage : MonoBehaviourPunCallbacks
{
    // 사망 후 투명 처리를 위한 MeshRenderer 컴포넌트의 배열
    private Renderer[] renderers;

    private int initHp = 100;
    private int curHp = 100;

    private Animator anim;
    private CharacterController controller;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashRespawn = Animator.StringToHash("Respawn");

    private GameManager gamemanager;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        curHp = initHp;

        gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(curHp > 0 && collision.collider.CompareTag("BULLET"))
        {
            curHp -= 20;
            if(curHp <= 0)
            {
                if(photonView.IsMine)
                {
                    int actorNum = collision.collider.GetComponent<Bullet>().actorNumber;

                    Player lastShootPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNum);

                    string msg = string.Format("\n<color=#00ff00>{0}</color> is killed by <color=#ff0000>{1}</color>",
                        photonView.Owner.NickName,
                        lastShootPlayer.NickName);
                    photonView.RPC(nameof(KillMessage), RpcTarget.AllBufferedViaServer, msg);
                }
                   
                StartCoroutine(nameof(PlayerDie));
            }
        }
    }

    [PunRPC]
    void KillMessage(string msg)
    {
        gamemanager.msgList.text += msg;
    }

    IEnumerator PlayerDie()
    {
        controller.enabled = false;
        anim.SetBool(hashRespawn, false);
        anim.SetTrigger(hashDie);

        yield return new WaitForSeconds(3.0f);

        anim.SetBool(hashRespawn, true);

        SetPlayerVisible(false);

        yield return new WaitForSeconds(1.5f);

        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        int idx = Random.Range(0, points.Length);
        transform.position = points[idx].position;

        curHp = 100;
        SetPlayerVisible(true);
        controller.enabled = true;
    }

    void SetPlayerVisible(bool visible)
    {
        for(int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].enabled = visible;
        }
    }
}

using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Movement : MonoBehaviourPunCallbacks, IPunObservable
{
    private CharacterController controller;
    private new Transform transform;
    private Animator animator;
    private new Camera camera;

    // ������ �÷��ο� ����ĳ���� �ϱ� ���� ����.
    private Plane plane;
    private Ray ray;
    private Vector3 hitPoint;

    public float moveSpeed = 10.0f;

    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");

    private new PhotonView photonView;
    private CinemachineVirtualCamera virtualCamera;

    private Vector3 receivePos;
    private Quaternion receiveRot;

    public float damping = 10.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();

        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        // PhotonView�� �ڽ��� ���� ���, �ó׸ӽ� ���� ī�޶� ����.
        if (photonView.IsMine)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }

        camera = Camera.main;
 
        // �� ����, �ٴ��� �����ϴ� ���⺤�͸� ���� Plane����.
        plane = new Plane(transform.up, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            Move();
            Turn();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position,
                receivePos,
                Time.deltaTime * damping);

            transform.rotation = Quaternion.Slerp(transform.rotation,
                receiveRot,
                Time.deltaTime * damping);
        }
    }

    void Move()
    {
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;

        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;

        Vector3 moveDir = (cameraForward * v) + (cameraRight * h);
        moveDir.Set(moveDir.x, 0.0f, moveDir.z);

        controller.SimpleMove(moveDir * moveSpeed);

        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }

    void Turn()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);
        float enter = 0.0f;

        plane.Raycast(ray, out enter);
        hitPoint = ray.GetPoint(enter);

        Vector3 lookDir = hitPoint - transform.position;
        lookDir.y = 0.0f;

        transform.localRotation = Quaternion.LookRotation(lookDir);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // �ڽ��� ���� ĳ������ ���, �ڽ��� �����͸� �ٸ� ��Ʈ��ũ �������� �۽�.
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
}

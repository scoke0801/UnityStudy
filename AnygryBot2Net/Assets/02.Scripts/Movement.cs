using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController controller;
    private new Transform transform;
    private Animator animator;
    private new Camera camera;

    // 가상의 플레인에 레이캐스팅 하기 위한 변수.
    private Plane plane;
    private Ray ray;
    private Vector3 hitPoint;

    public float moveSpeed = 10.0f;

    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");

    private PhotonView photonView;
    private CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();

        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        // PhotonView가 자신의 것일 경우, 시네머신 가상 카메라를 연결.
        if (photonView.IsMine)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }

        camera = Camera.main;
 
        // 한 점과, 바닥이 생성하는 방향벡터를 통해 Plane정의.
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
}

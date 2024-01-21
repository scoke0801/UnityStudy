using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ī�޶� �Ӽ� �� �߿� �Ӽ� �ϳ��� ī�޶�κ��� ������ ����, �Ǻ� ������ ����
/// ��ġ ������ ���ʹ� �浹 ó�������� ����ϰ� �Ǻ� ������ ���ʹ� �ü� �̵��� ����ϵ���
/// �浹üũ : ���� �浹 üũ ���( ĳ���ͷκ��� ī�޶�, ī�޶�κ��� ĳ���� ���� )
/// recoil( ��� �ݵ� ) �� ���� ���
/// FOV ���� ���.
/// </summary>
public class ThirdPirsonOrbitCamera : MonoBehaviour
{
    public Transform player; // player Transform
    public Vector3 pivotOffset = new Vector3(0.0f, 1.0f, 0.0f);
    public Vector3 camOffset = new Vector3(0.4f, 0.5f, -2.0f);

    public float smooth = 10.0f; // ī�޶� ���� �ӵ�.
    public float horizontalAimingSpeed = 6.0f; // ���� ȸ�� �ӵ�
    public float verticalAimingSpeed = 6.0f; // ���� ȸ�� �ӵ�
    public float maxVerticalAngle = 30.0f;  //ī�޶� ���� �ִ� ����.
    public float minVerticalAngle = -60.0f; // ī�޶��� ���� �ּ� ����

    public float recoilAngleBounce = 5.0f;// ��� �ݵ� �ٿ ��.
    private float angleH = 0.0f;    // ���콺 �̵��� ���� ī�޶� �����̵� ��ġ.
    private float angleV = 0.0f;    // ���콺 �̵��� ���� ī�޶� �����̵� ��ġ.

    private Transform cameraTransform; // Ʈ������ ĳ��.
    private Camera myCamera;
    private Vector3 relCameraPos;   // �÷��̾�κ��� ī�޶�"������" ����.
    private float relCameraPosMag;  // �÷��̾�κ��� ī�޶�"������"�Ÿ�.

    private Vector3 smoothPivotOffset;  // ī�޶� �Ǻ� ������ ����.
    private Vector3 smoothCamOffset;    // ī�޶� ��ġ ������ ����.

    private Vector3 targetPivotOffset;  // ī�޶� �Ǻ��� ������ ����.
    private Vector3 targetCamOffset;    // ī�޶� ��ġ ������ ����.

    private float defaultFOV;           // �⺻ �þ߰�.
    private float targetFOV;            // Ÿ�� �þ߰�.
    private float targetMaxVerticalAngle;   // ī�޶� ���� �ִ� ����.
    private float recoilAngle = 0.0f;   // ��� �ݵ� ����.

    public float GetH
    {
        get
        {
            return angleH;
        }
    }
}

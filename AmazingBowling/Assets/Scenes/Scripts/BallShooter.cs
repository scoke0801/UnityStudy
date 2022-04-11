using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BallShooter : MonoBehaviour
{
    public CamFollow cam;

    public Rigidbody ball;

    public Transform firePosition;

    public Slider powerSlider;

    public AudioSource shootingAudio;
    
    // �߻� �� ��
    public AudioClip fireClip;

    // ���� ������ ����
    public AudioClip chargingClip;

    public float minForce = 15.0f;
    public float maxForce = 30.0f;

    public float chargingTime = 0.75f;

    private float currentForce;
    private float chargeSpeed;
    private bool fired;

    private void OnEnable()
    {
        currentForce = minForce;
        powerSlider.value = minForce;
        fired = false;
    }

    private void Start()
    {
        chargeSpeed = (maxForce - minForce) / chargingTime;
    }
    private void Update()
    {
        if( fired)
        {
            // �ѹ��̶� �߻������� �� �̻� ���� ���ϵ���
            return;
        }
        powerSlider.value = minForce;

        if (currentForce >= maxForce && !fired)
        {
            currentForce = maxForce;
            Fire();
            // �߻�ó�� 
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            // ��ư�� ���� ���� 
            currentForce = minForce;
            shootingAudio.clip = chargingClip;
            shootingAudio.Play();
        }
        else if(Input.GetButton("Fire1") && !fired)
        { 
            // ��ư�� ������ �ִ� ����
            currentForce = currentForce + chargeSpeed * Time.deltaTime;
            
            powerSlider.value = currentForce;
        }
        else if(Input.GetButtonUp("Fire1") && !fired)
        { 
            // ���� �߻� ó��
            Fire();
        }
    }

    private void Fire()
    {
        fired = true;

        Rigidbody ballInstance = Instantiate(ball, firePosition.transform.position, firePosition.rotation);
        ballInstance.velocity = currentForce * firePosition.forward;

        shootingAudio.clip = fireClip;
        shootingAudio.Play();

        currentForce = minForce;

        cam.SetTarget(ballInstance.transform, CamFollow.State.Tracking);
    }
}

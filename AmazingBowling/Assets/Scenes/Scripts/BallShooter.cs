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
    
    // 발사 할 때
    public AudioClip fireClip;

    // 힘을 모으는 동안
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
            // 한번이라도 발사했으면 더 이상 동작 안하도록
            return;
        }
        powerSlider.value = minForce;

        if (currentForce >= maxForce && !fired)
        {
            currentForce = maxForce;
            Fire();
            // 발사처리 
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            // 버튼을 누른 순간 
            currentForce = minForce;
            shootingAudio.clip = chargingClip;
            shootingAudio.Play();
        }
        else if(Input.GetButton("Fire1") && !fired)
        { 
            // 버튼을 누르고 있는 동안
            currentForce = currentForce + chargeSpeed * Time.deltaTime;
            
            powerSlider.value = currentForce;
        }
        else if(Input.GetButtonUp("Fire1") && !fired)
        { 
            // 실제 발사 처리
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

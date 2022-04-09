using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterRotator : MonoBehaviour
{
    private enum RotateState
    {
        Idle, Vertical, Horizontal, Ready
    }

    private RotateState state;

    public float verticalRotateSpeed = 360.0f;
    public float horizontalRotateSpeed = 360.0f;

    // Start is called before the first frame update
    void Start()
    {
        state = RotateState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        // edit->project settings->InputManager->axex-> ¼±Á¤ÀÇ
        if (Input.GetButton("Fire1"))
        { 
            if (state == RotateState.Idle)
            { 
                state = RotateState.Horizontal;
            }
            else if (state == RotateState.Horizontal)
            {
                transform.Rotate(new Vector3(0, horizontalRotateSpeed * Time.deltaTime, 0));
            }
            else if (state == RotateState.Vertical)
            {
                transform.Rotate(new Vector3(-verticalRotateSpeed * Time.deltaTime, 0, 0));
            }
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            if (state == RotateState.Horizontal)
            {
                state = RotateState.Vertical;
            }
            else if (state == RotateState.Vertical)
            {
                state = RotateState.Ready;
            }
        }
    }
}

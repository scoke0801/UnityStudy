using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyPush : MonoBehaviour
{
    public LayerMask pushLayers;
    public bool canPush;

    [Range(0.5f, 5f), Tooltip("미는 힘")]
    public float strength = 1.1f;

    [Tooltip("플레이어 아래의 대상은 밀지 않음")]
    public float minPushHeight = -0.3f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (canPush)
        {
            PushRigidBodies(hit);
        }
    }

    private void PushRigidBodies(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if( body == null || body.isKinematic) { return; }

        LayerMask bodyLayerMask = 1 << body.gameObject.layer;
        if ((bodyLayerMask & pushLayers) == 0) { return; }

        if ( hit.moveDirection.y < minPushHeight) { return; }

        if( body.mass > strength) { return; }

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

        body.AddForce(pushDir * strength, ForceMode.Impulse);
    }
}

using System;
using UnityEngine;
[RequireComponent(typeof(PlayerController))]

public class GroundPosition : MonoBehaviour
{
    public RaycastHit GroundRaycastHit;
    public Transform GroundPointTransform;
    public bool groundedState;
    private LayerMask GroundMask;
    void Awake()
    {
        GroundMask = LayerMask.GetMask("Ground");
        
    }
    void OnEnable()
    {
        GroundPointTransform = GetComponent<Transform>().Find("GroundBelowPlayer").transform;
        gameObject.GetComponent<PlayerController>().isGrounded = groundedState;
    }

    
    void FixedUpdate()
    {
        Physics.Raycast(transform.position, Vector3.down, out GroundRaycastHit, 20.0f, GroundMask);
        GroundPointTransform.position = GroundRaycastHit.point;
        groundedState = GroundedState(1.6f);
        //Debug.Log(GroundPointTransform.position);
    }

    public bool GroundedState(float maxDistanceAllowed = 0.05f)
    {
        return (Math.Abs(transform.position.y - GroundPointTransform.position.y) <= maxDistanceAllowed) ? true : false;
        //if (transform.position.y )
    }
}

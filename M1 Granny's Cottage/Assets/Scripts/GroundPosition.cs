using UnityEngine;

public class GroundPosition : MonoBehaviour
{
    public RaycastHit GroundRaycastHit;
    public Transform GroundPointTransform;
    LayerMask GroundMask;
    void Awake()
    {
        GroundMask = LayerMask.GetMask("Ground");
    }
    void OnEnable()
    {
        GroundPointTransform = GetComponent<Transform>().Find("GroundBelowPlayer").transform;
        
        
    }

    
    void FixedUpdate()
    {
        Physics.Raycast(transform.position, Vector3.down, out GroundRaycastHit, 20.0f, GroundMask);
        GroundPointTransform.position = GroundRaycastHit.point;
        //Debug.Log(GroundPointTransform.position);
    }
}

using System;
using UnityEngine;

public class SceneryTransparency : MonoBehaviour
{
    private GameObject gameCamera;
    private Vector3 cameraPosition;
    private GameObject grannyObject;
    private Vector3 grannyPosition;
    private Collider[] obstructingScenery = new Collider[200]; // arbitrary number picked for now
    private LayerMask sceneryLayerMask;
    public static Collider[] FullSceneryArray;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        gameCamera = FindAnyObjectByType<PlayerCameraScript>().gameObject;
        grannyObject = FindAnyObjectByType<PlayerController>().gameObject;
        sceneryLayerMask = LayerMask.GetMask("Scenery");
        obstructingScenery = new Collider[FullSceneryArray.Length/4];
    }

    // Update is called once per frame
    void Update()
    {
        cameraPosition = gameCamera.transform.position;
        grannyPosition = grannyObject.transform.position;
        DetectOverlap();
    }

    // 
    void DetectOverlap()
    {
        if (Physics.OverlapCapsuleNonAlloc(cameraPosition,grannyPosition, 2.0f, obstructingScenery, sceneryLayerMask) 
            > 0)
        {
            foreach (Collider overlappingObject in obstructingScenery)
            {
                overlappingObject.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < FullSceneryArray.Length; i++)
            {
                foreach (Collider overlappingObject in obstructingScenery)
                {
                    if (!Array.ReferenceEquals(FullSceneryArray[i], overlappingObject))
                    {
                        FullSceneryArray[i].gameObject.SetActive(true);
                    }
                }
                
            }
        }

    }
}

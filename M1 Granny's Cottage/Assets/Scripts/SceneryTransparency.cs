using System;
using UnityEditor;
using UnityEngine;

// Applied to the Cinemachine Camera to manage which objects need do become transparified
public class SceneryTransparency : MonoBehaviour
{
    private GameObject gameCamera;
    private Vector3 cameraPosition;
    private GameObject grannyObject;
    private Vector3 grannyPosition;
    private Collider[] obstructingScenery;
    private LayerMask sceneryLayerMask;
    public static SceneryObject[] FullSceneryArray;

    private const float DETECTION_CAPSULE_RADIUS = 2.4f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get camera and granny objects for later position information
        gameCamera = FindAnyObjectByType<PlayerCameraScript>().gameObject;
        grannyObject = FindAnyObjectByType<PlayerController>().gameObject;
        sceneryLayerMask = LayerMask.GetMask("Scenery");

        // make a defined size array to store colliders in to help with memory management
        if (FullSceneryArray != null && FullSceneryArray.Length > 0)
        {
            obstructingScenery = new Collider[Mathf.Max(1, FullSceneryArray.Length / 4)];
        }
        else
        {
            obstructingScenery = new Collider[50];
        }
    }

    // Update is called once per frame
    void Update()
    {
        cameraPosition = gameCamera.transform.position;
        grannyPosition = grannyObject.transform.position;
        DetectOverlap();
    }

    // check for colliders in the Scenery Layer with SceneryObject scripts and run transparification routine
    void DetectOverlap()
    {

        if (Physics.OverlapCapsuleNonAlloc(cameraPosition,grannyPosition, DETECTION_CAPSULE_RADIUS, obstructingScenery, sceneryLayerMask) 
            > 0)
        {
            
            foreach (Collider overlappingObject in obstructingScenery)
            {
                if (overlappingObject == null) continue;

                SceneryObject scenery = overlappingObject.GetComponent<SceneryObject>();
                if (scenery == null) continue;

                //Debug.Log(overlappingObject.gameObject);
                StartCoroutine(SceneryObject.Transparify(overlappingObject));
                
            }

            Array.Clear(obstructingScenery, 0, obstructingScenery.Length);
        }
        

    }
}

using System;
using UnityEditor;
using UnityEngine;


public class SceneryTransparency : MonoBehaviour
{
    private GameObject gameCamera;
    private Vector3 cameraPosition;
    private GameObject grannyObject;
    private Vector3 grannyPosition;
    private Collider[] obstructingScenery;// = new SceneryObject[200]; // arbitrary number picked for now
    private LayerMask sceneryLayerMask;
    public static SceneryObject[] FullSceneryArray;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        gameCamera = FindAnyObjectByType<PlayerCameraScript>().gameObject;
        grannyObject = FindAnyObjectByType<PlayerController>().gameObject;
        sceneryLayerMask = LayerMask.GetMask("Scenery");
        obstructingScenery = new Collider[FullSceneryArray.Length/4];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cameraPosition = gameCamera.transform.position;
        grannyPosition = grannyObject.transform.position;
        DetectOverlap();
    }

    // 
    void DetectOverlap()
    {
        /*foreach (SceneryObject potentialObstruction in FullSceneryArray)
        {
            potentialObstruction.obstructionState = SceneryObject.ObstructionState.NONBLOCKING;
        }*/
        
        //ArrayUtility.Clear(ref obstructingScenery);

        if (Physics.OverlapCapsuleNonAlloc(cameraPosition,grannyPosition, 4.0f, obstructingScenery, sceneryLayerMask) 
            > 0)
        {
            
            foreach (Collider overlappingObject in obstructingScenery)
            {
                Debug.Log(overlappingObject.gameObject);
                StartCoroutine(SceneryObject.Transparify(overlappingObject));
                //SceneryObject overlapSceneryOBJ = overlappingObject.componen//gameObject.GetComponent<SceneryObject>();
                //Debug.Log(overlapSceneryOBJ);
                //overlapSceneryOBJ.obstructionState = SceneryObject.ObstructionState.BLOCKING;
                //overlapSceneryOBJ.Transparify();
            }


        }

    }
}

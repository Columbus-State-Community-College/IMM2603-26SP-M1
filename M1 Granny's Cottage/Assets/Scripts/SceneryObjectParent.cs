using System;
using UnityEngine;

public class SceneryObjectParent : MonoBehaviour
{
    private SceneryObject[] childObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        childObjects = GetComponentsInChildren<SceneryObject>();
        
        //Debug.Log("Number of Scenery Objects: " + childObjects.Length);
        SceneryTransparency.FullSceneryArray = childObjects;
    }

    

}

using System;
using UnityEngine;

public class SceneryObjectParent : MonoBehaviour
{
    private Collider[] childObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        childObjects = this.GetComponentsInChildren<Collider>();
        Debug.Log("Number of Scenery Objects: " + childObjects.Length);
        SceneryTransparency.FullSceneryArray = childObjects;
    }

}

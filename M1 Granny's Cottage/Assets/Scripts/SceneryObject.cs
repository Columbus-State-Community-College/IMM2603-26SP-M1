using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

// applied to any object on the Scenery layer that needs to be 'transparified'
public class SceneryObject : MonoBehaviour
{
    
    public enum ObstructionState
    {
        NONBLOCKING,
        BLOCKING,
        TRANSPARIFIED
    }

    public ObstructionState obstructionState = ObstructionState.NONBLOCKING;

    public Coroutine TransparifyCoroutine;

    // the below is for potential polish to fix the flicker
    //private SceneryObject passedOBJ;

    /*
    void Update()
    {
        if (this.obstructionState == ObstructionState.NONBLOCKING)
        {
            
        }
    }*/
    

    public static IEnumerator Transparify(Collider collider)
    {

        if (collider == null)
            yield break;

        // get the parent gameObject
        SceneryObject passedOBJ = collider.gameObject.GetComponent<SceneryObject>();

        if (passedOBJ == null)
        {    
            yield break;
        }

        if (passedOBJ.obstructionState == ObstructionState.TRANSPARIFIED) 
        {
            yield break;
        }

        passedOBJ.obstructionState = ObstructionState.BLOCKING;

        // get ALL renderers of any type under the parent object
        Renderer[] renderers = passedOBJ.GetComponentsInChildren<Renderer>();

        //Debug.Log("Renderers List: " + renderers);

        // set the color of each object to what it already is, except with 0.15 alpha value
        foreach (Renderer renderer in renderers)
        {
            //Debug.Log(renderer.name);
            if (renderer == null)
            {
                Debug.Log(passedOBJ + "Produced null renderer");
                yield break;
            }
            
            Color matColor = renderer.material.color;
            Color transparifiedColor = new Color(matColor.r, matColor.g, matColor.b, 0.15f);
            renderer.material.SetColor("_BaseColor", transparifiedColor);
 
        }

        //Debug.Log(passedOBJ + " Transparified for 2 seconds");
        passedOBJ.obstructionState = ObstructionState.TRANSPARIFIED;
        yield return new WaitForSeconds(3.5f);

        // set the color of each object to what it already is, except with 1.0 alpha value
        foreach (Renderer renderer in renderers)
        {
            //Debug.Log(renderer.name);
            if (renderer == null)
            {
                Debug.Log(passedOBJ + "Produced null renderer");
                yield break;
            }
            
            //Debug.Log(passedOBJ.gameObject.name);
            Color matColor = renderer.material.color;
            Color originalColor = new Color(matColor.r, matColor.g, matColor.b, 1.0f);
            renderer.material.SetColor("_BaseColor", originalColor);
             
        }

        //passedOBJ.StartCoroutine(passedOBJ.Occlude(renderers));
        passedOBJ.obstructionState = ObstructionState.NONBLOCKING;
        yield return null;

    }

    /*
    public IEnumerator Occlude(Renderer[] renderers)
    {
        passedOBJ.obstructionState = ObstructionState.NONBLOCKING;
        yield return null;
    }
    */
}    

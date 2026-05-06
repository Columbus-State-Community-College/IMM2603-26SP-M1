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
        TRANSPARIFIED,
        RECENTLY_TRANSPARIFIED
    }

    public ObstructionState obstructionState = ObstructionState.NONBLOCKING;
    public ObstructionState previousObstructionState;

    public Coroutine TransparifyCoroutine;

    //public Coroutine FadeCoroutine;

    public Coroutine OccludeCoroutine;

    public WaitForSeconds waitForSeconds = new WaitForSeconds(3.5f);

    public Renderer[] renderersArray;
    public SceneryObject coroutineOBJ;

    // the below is for potential polish to fix the flicker
    //private SceneryObject passedOBJ;

    
    void Update()
    {
        /*
        try
        {
            foreach (Renderer renderer in renderersArray)
            {
                if (this.obstructionState == ObstructionState.NONBLOCKING 
                && renderersArray[0].material.color.a == 0.15f)
                {
                    coroutineOBJ.Occlude(coroutineOBJ, renderersArray);
                }
                else if(this.obstructionState == ObstructionState.TRANSPARIFIED 
                && renderersArray[0].material.color.a == 1.0f)
                {
                    coroutineOBJ.Fade(coroutineOBJ, renderersArray);
                }
            }
            
        }
        catch (System.IndexOutOfRangeException)
        {
            // keep going.
        }
        */
    }
    

    public static IEnumerator Transparify(Collider collider)
    {

        if (collider == null)
        {
            yield break;
        }
        // get the parent gameObject
        SceneryObject passedOBJ = collider.gameObject.GetComponent<SceneryObject>();
        passedOBJ.coroutineOBJ = passedOBJ;
        if (passedOBJ == null)
        {    
            yield break;
        }

        if (passedOBJ.coroutineOBJ.obstructionState == ObstructionState.TRANSPARIFIED) 
        {
            yield break;
        }

        passedOBJ.coroutineOBJ.obstructionState = ObstructionState.BLOCKING;

        // get ALL renderers of any type under the parent object
        Renderer[] renderers = passedOBJ.GetComponentsInChildren<Renderer>();
        passedOBJ.renderersArray = renderers;
        
        passedOBJ.Fade(passedOBJ.coroutineOBJ, passedOBJ.renderersArray);

        /*
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
        */

        //Debug.Log(passedOBJ + " Transparified for ??? seconds");
        passedOBJ.coroutineOBJ.obstructionState = ObstructionState.TRANSPARIFIED;
        yield return passedOBJ.waitForSeconds;

        passedOBJ.Occlude(passedOBJ.coroutineOBJ, passedOBJ.renderersArray);

        /*
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
        */

        //passedOBJ.StartCoroutine(passedOBJ.Occlude(renderers));
        passedOBJ.obstructionState = ObstructionState.NONBLOCKING;
        yield return null;

    }

    
    public void Fade(SceneryObject sceneOBJ, Renderer[] renderers)
    {
        sceneOBJ.obstructionState = ObstructionState.NONBLOCKING;
        // set the color of each object to what it already is, except with 0.15 alpha value
        foreach (Renderer renderer in renderers)
        {
            //Debug.Log(renderer.name);
            if (renderer == null)
            {
                Debug.Log(sceneOBJ + "Produced null renderer");
                break;
            }
            
            Color matColor = renderer.material.color;
            Color transparifiedColor = new Color(matColor.r, matColor.g, matColor.b, 0.15f);
            renderer.material.SetColor("_BaseColor", transparifiedColor);
        }
        //
    }

    public void Occlude(SceneryObject sceneOBJ, Renderer[] renderers)
    {
        sceneOBJ.obstructionState = ObstructionState.NONBLOCKING;
        // set the color of each object to what it already is, except with 0.15 alpha value
        foreach (Renderer renderer in renderers)
        {
            //Debug.Log(renderer.name);
            if (renderer == null)
            {
                Debug.Log(sceneOBJ + "Produced null renderer");
                break;
            }
            
            Color matColor = renderer.material.color;
            Color transparifiedColor = new Color(matColor.r, matColor.g, matColor.b, 1.0f);
            renderer.material.SetColor("_BaseColor", transparifiedColor);
        }
        //
    }
    
}    

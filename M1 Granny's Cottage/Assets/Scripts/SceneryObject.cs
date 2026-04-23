using UnityEngine;
using System.Collections;

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

    public static IEnumerator Transparify(Collider collider)
    {
        //Debug.Log("Transparify Coroutine reached for "+ collider.gameObject);
        if (collider == null)
            yield break;

        SceneryObject passedOBJ = collider.gameObject.GetComponent<SceneryObject>();
        //Debug.Log("Transparify Coroutine reached for "+ passedOBJ);

        if (passedOBJ == null)
        {    
            yield break;
        }

        if (passedOBJ.obstructionState == ObstructionState.TRANSPARIFIED) 
        {
            //Debug.Log(passedOBJ + " is transparified.");
            yield break;
        }


        passedOBJ.obstructionState = ObstructionState.BLOCKING;
        MeshRenderer[] renderers = passedOBJ.GetComponentsInChildren<MeshRenderer>();
        

        foreach (MeshRenderer renderer in renderers)
        {
            Debug.Log(renderer.name);
            if (renderer == null)
            {
                Debug.Log(passedOBJ + "Produced null renderer");
                yield break;
            }
            
            
            //Debug.Log(passedOBJ.gameObject.name);
            Color matColor = renderer.material.color;
            Color OGColor = matColor;
            matColor.a = 0.5f;
            renderer.material.SetColor("transparified", matColor);
            passedOBJ.obstructionState = ObstructionState.TRANSPARIFIED; 
        }

        passedOBJ.obstructionState = ObstructionState.TRANSPARIFIED;
        yield return new WaitForSeconds(2);

        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer == null)
            {
                Debug.Log(passedOBJ + "Produced null renderer");
                yield break;
            }
            
            
            //Debug.Log(passedOBJ.gameObject.name);
            Color matColor = renderer.material.color;
            Color OGColor = matColor;
            matColor.a = 1.0f;
            renderer.material.SetColor("normal", matColor);
        }
        
        passedOBJ.obstructionState = ObstructionState.NONBLOCKING; 
        yield return null;

    }
    
}    

using UnityEngine;
using System.Collections;
using System.Linq;

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

        if (passedOBJ.obstructionState == ObstructionState.TRANSPARIFIED || passedOBJ.obstructionState == ObstructionState.NONBLOCKING) 
        {
            //Debug.Log(passedOBJ + " is transparified.");
            yield break;
        }

        // uncomment the above nonblocking added to pass bugged current state

        passedOBJ.obstructionState = ObstructionState.BLOCKING;
        MeshRenderer[] renderers = passedOBJ.GetComponentsInChildren<MeshRenderer>();
        Debug.Log("Renderers List: " + renderers);
        Color[] colors = new Color[renderers.Length];
        

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
            colors.Append(matColor);
            Color transparifiedColor = new Color(matColor.r, matColor.g, matColor.b, 0.1f);
            renderer.material.SetColor("_BaseColor", transparifiedColor);
            Debug.Log(renderer.material.color);
            //passedOBJ.obstructionState = ObstructionState.TRANSPARIFIED; 
        }

        //Debug.Log(passedOBJ + " Transparified for 2 seconds");
        passedOBJ.obstructionState = ObstructionState.TRANSPARIFIED;
        yield return new WaitForSeconds(2);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null)
            {
                Debug.Log(passedOBJ + "Produced null renderer");
                yield break;
            }

            renderers[i].material.SetColor("_BaseColor", colors.ElementAt(i));
        }
        
        passedOBJ.obstructionState = ObstructionState.NONBLOCKING; 
        yield return null;

    }
    
}    

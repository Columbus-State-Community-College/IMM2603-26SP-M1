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
        if (collider == null)
            yield break;

        SceneryObject passedOBJ = collider.gameObject.GetComponent<SceneryObject>();

        if (passedOBJ == null)
            yield break;

        if (passedOBJ.obstructionState == ObstructionState.TRANSPARIFIED ||
            passedOBJ.obstructionState == ObstructionState.NONBLOCKING) {yield break;}
        
        MeshRenderer renderer = passedOBJ.GetComponentInChildren<MeshRenderer>();

        if (renderer == null)
             yield break;

        passedOBJ.obstructionState = ObstructionState.BLOCKING;
        Debug.Log(passedOBJ.gameObject.name);
        Color matColor = passedOBJ.gameObject.GetComponent<MeshRenderer>().material.color;
        Color OGColor = matColor;
        matColor.a = 0.5f;
        passedOBJ.gameObject.GetComponent<MeshRenderer>().material.SetColor("transparified", matColor);
        passedOBJ.obstructionState = ObstructionState.TRANSPARIFIED; 
        yield return new WaitForSeconds(2);
        passedOBJ.gameObject.GetComponent<MeshRenderer>().material.SetColor("normal", matColor);
        passedOBJ.obstructionState = ObstructionState.NONBLOCKING; 
        yield return null;
    }
    
}    

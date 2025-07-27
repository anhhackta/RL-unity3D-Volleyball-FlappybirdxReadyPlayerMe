using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSet : MonoBehaviour
{
    public void ResetPosition()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<PipeBehavior>().Initialize();
        }
    }

    public Transform GetNextPipe()
    {
        float adjvalue = float.MaxValue;
        Transform mostAdjacent = null;
        foreach (Transform child in transform)
        {
            float childX = child.localPosition.x;
            if (childX < adjvalue && childX > -.3f)
            {
                mostAdjacent = child;
                adjvalue = child.localPosition.x;
            }
        }
        return mostAdjacent;
    }
}

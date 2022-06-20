using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLayer
{
    public void CangeTransformLayer(Transform parent, string name)
    {
        parent.gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in parent)
            CangeTransformLayer(child, name);
    }
}

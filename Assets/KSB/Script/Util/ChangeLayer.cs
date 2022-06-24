using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLayer
{
    public void CangeTransformLayer(Transform parent, string name, bool ischild)
    {
        parent.gameObject.layer = LayerMask.NameToLayer(name);
        if (ischild)
            foreach (Transform child in parent)
                CangeTransformLayer(child, name, ischild);
    }
}

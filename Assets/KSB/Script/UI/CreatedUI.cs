using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatedUI : MonoBehaviour
{
    private void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resolution : MonoBehaviour
{
    void OnPreCull() => GL.Clear(true, true, Color.black);
}

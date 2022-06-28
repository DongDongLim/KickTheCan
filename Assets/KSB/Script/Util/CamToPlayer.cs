using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamToPlayer : MonoBehaviour
{
    [SerializeField]
    private Camera mainCam;

    [SerializeField]
    private GameObject player;

    private void Start()
    {
        player = DH.CameraMng.instance.playerObject;
    }

    private void Update()
    {
        
    }
}

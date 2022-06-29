using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamToPlayer : MonoBehaviour
{
    [SerializeField]
    private Camera mainCam;

    [SerializeField]
    private Transform player;

    private RaycastHit[] TransparentHits;

    private void Start()
    {
        player = DH.CameraMng.instance.playerObject.transform;
    }

    private void Update()
    {
        
    }

    public void CamRayToChar()
    {
        // 캐릭터 Pos
        Vector3 CharPos = player.position;

        // 카메라와 캐릭터의 거리
        float Distance = Vector3.Distance(transform.position, player.transform.position);

        // 카메라와 캐릭터의 방향
        Vector3 DirToCam = (transform.position - CharPos).normalized;

        TransparentHits = Physics.RaycastAll(CharPos, DirToCam, Distance);
        Debug.DrawRay(CharPos, Distance * DirToCam, Color.red, 0.1f);

        for (int i = 0; i < TransparentHits.Length; i++)
        {
            RaycastHit hit = TransparentHits[i];

            //Debug.Log(hit.collider.gameObject.transform.parent.transform.GetComponentInChildren<MeshRenderer>().name);
            //objectRender = 
            //if (objectRender != null)
            //{
                

            //    Debug.Log("렌더러 받아옴");
            //}
        }
    }
}

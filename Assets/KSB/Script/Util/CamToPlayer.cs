using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamToPlayer : MonoBehaviour
{

    [SerializeField]
    private Camera mainCam;

    [SerializeField]
    private Transform player;
    private Vector3 offset;
    private Vector3 charPos;

    [SerializeField]
    private Transform[] extraPos;

    private RaycastHit[] mainRayHits;
    private RaycastHit[] extraRayHits;

    List<Collider> tempc = new List<Collider>();

    MeshRenderer objectRender;
    public Material transparentMaterial;

    private void Start()
    {
        mainCam = GetComponent<Camera>();
        DH.CameraMng.instance.playerSetAction += SetPlayer;
        extraPos = GetComponentsInChildren<Transform>();
        offset = new Vector3(0, 1f, 0);
    }

    private void FixedUpdate()
    {
        CamRayToChar();
    }

    public void SetPlayer()
    {
        player = DH.CameraMng.instance.playerObject.transform;
    }

    public void CamRayToChar()
    {
        if (null == player)
            return;

        charPos = player.position + offset;

        // 카메라와 캐릭터의 거리
        float Distance = Vector3.Distance(player.position,transform.position );

        // 카메라와 캐릭터의 방향
        Vector3 Direction = (charPos - transform.position).normalized;

        // 레이어마스크
        int layerMask = 1 << LayerMask.NameToLayer("Wall");

        // 시작위치, 방향, 거리, 레이어
        mainRayHits = Physics.RaycastAll(mainCam.transform.position, Direction, Distance, layerMask);
        Debug.DrawRay(mainCam.transform.position, Distance * Direction, Color.red, 0.1f);

        

        for (int i = 0; i < mainRayHits.Length; i++)
        {
            RaycastHit mainhits = mainRayHits[i];
            tempc.Add(mainhits.collider);

            objectRender = mainhits.collider.gameObject.GetComponent<MeshRenderer>();

            if (objectRender != null)
            {
                Debug.Log("렌더러 받아옴1");
                Debug.Log(tempc.Count);
                transparentMaterial = objectRender.material;
                transparentMaterial.SetColor("_Color", new Color(1f,1f,1f,0f));
            }
        }

        for (int i = 1; i < extraPos.Length; i++)
        {
            extraRayHits = Physics.RaycastAll(extraPos[i].transform.position, Direction, Distance, layerMask);
            Debug.DrawRay(extraPos[i].transform.position, Distance * Direction, Color.yellow, 0.1f);
            Debug.Log(extraRayHits.Length);
            for (int j = 0; j < extraRayHits.Length; j++)
            {
                RaycastHit extrahits = extraRayHits[j];

                if (tempc.Contains(extrahits.collider) == true)
                {
                    Debug.Log("tempc continue");
                    continue;
                }  

                objectRender = extrahits.collider.gameObject.GetComponent<MeshRenderer>();

                if (objectRender != null)
                {
                    Debug.Log("렌더러 받아옴2");
                    transparentMaterial = objectRender.material;
                    transparentMaterial.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
                }
            }

        }
        tempc.Clear();
    }
}

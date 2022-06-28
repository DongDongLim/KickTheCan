using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TaggerAttack : MonoBehaviour
{
    [SerializeField]
    GameObject attackColl;


    public DH.TaggerController controller;

    private void Awake() {
        controller = GetComponentInParent<DH.TaggerController>();
    }

    public void AttackDiminish()
    {
        controller.AttackComplete();
    }

    public void JumpStart()
    {
        KSB.VFXMng.instance.OnDust(transform);
    }

    [PunRPC]
    public void Attack()
    {
        attackColl.SetActive(true);
        StartCoroutine("AttackEnd");
    }

    [PunRPC]
    IEnumerator AttackEnd()
    {
        yield return new WaitForSeconds(0.1f);
        attackColl.SetActive(false);
    }
}

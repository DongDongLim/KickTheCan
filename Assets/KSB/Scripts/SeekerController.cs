using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
public class SeekerController : MonoBehaviourPun
{
    private Rigidbody rigid;
    private Animator anim;
    public GameObject attackColl;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update() {
        Move();
        Jump();
    }

    [PunRPC]
    private void Move(){

    }
    
    [PunRPC]
    private void Jump(){

    }

    [PunRPC]
    private void Attack(){
        attackColl.SetActive(true);
        StartCoroutine("AttackEnd");
    }

    IEnumerator AttackEnd(){
        yield return new WaitForSeconds(0.1f);
    }
}

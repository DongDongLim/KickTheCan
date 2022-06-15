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

    private float hAxis = 0f;
    private float vAxis = 0f;
    private float moveSpeed = 5f;
    private float jumpPower = 5f;
    private Vector3 moveVec;    // 플레이어 움직이는 방향

    private bool isGrounded;		        // 땅에 서있는지 체크하기 위한 bool값
    public float groundDistance = 0.5f;		// Ray를 쏴서 검사하는 거리

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update() {
        Move();
        Jump();
        Attack();
    }

    [PunRPC]
    private void Move(){
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");

        moveVec = new Vector3(hAxis,0,vAxis).normalized;
        transform.LookAt(transform.position + moveVec);

        transform.position += moveVec * moveSpeed * Time.deltaTime;
    }
    
    [PunRPC]
    private void Jump(){
        if (!isGrounded)
            return;

        if (!Input.GetButtonDown("Jump"))
            return;
        
        rigid.AddForce(Vector3.up * jumpPower,ForceMode.Impulse);
    }

    [PunRPC]
    private void Attack(){
        if (!Input.GetButtonDown("Fire1"))
            return;
        
        attackColl.SetActive(true);
        StartCoroutine("AttackEnd");
    }

    IEnumerator AttackEnd(){
        yield return new WaitForSeconds(0.1f);
        attackColl.SetActive(false);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Ground"){
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        if (other.gameObject.tag == "Ground"){
            isGrounded = false;
        }
    }
}

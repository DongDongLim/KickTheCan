using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class SeekerController : MonoBehaviourPun
{
    [SerializeField]
    private Transform charactorBody;
    
    [SerializeField]
    private Transform cameraArm;

    [SerializeField]
    private GameObject attackColl;

    // 컴포넌트
    private Animator animator;
    private Rigidbody rigid;

    // 이동
    private Vector2 moveInput;
    private bool isMove;
    public float moveSpeed;

    // 점프
    private bool isJump;
    public float jumpPower;

    // 카메라 이동
    public float rotateSpeed;

    private void Start() {
        animator = charactorBody.GetComponent<Animator>() == null ? null : charactorBody.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    [PunRPC]
    public void Move(Vector2 inputDirection){
        moveInput = inputDirection;
        isMove = moveInput.magnitude != 0;

        Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
            
        animator?.SetBool("isMove", isMove);

        if (!isMove)
            return;

        charactorBody.forward = moveDir;
        rigid.MovePosition(transform.position + moveDir * Time.deltaTime * moveSpeed);
    }

    [PunRPC]
    public void LookAround(Vector2 inputDirection){
        // 마우스 이동 값 검출
        Vector2 mouseDelta = inputDirection * rotateSpeed;
        // 카메라의 원래 각도를 오일러 각으로 저장
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        // 카메라의 피치 값 계산
        float x = camAngle.x - mouseDelta.y;

        // 카메라 피치 값을 위쪽으로 70도 아래쪽으로 25도 이상 움직이지 못하게 제한
        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        // 카메라 암 회전 시키기
        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    [PunRPC]
    public void Jump(){
        if (!Input.GetButtonDown("Jump"))
            return; 

        if (isJump)
            return;

        rigid.AddForce(Vector3.up * jumpPower,ForceMode.Impulse);

        animator?.SetBool("isMove", false);

        isJump = true;
        animator?.SetBool("isJump", isJump);
    }

    [PunRPC]
    public void Attack(){
        attackColl.SetActive(true);
        animator?.SetTrigger("isAttack");
        StartCoroutine("AttackEnd");
    }

    IEnumerator AttackEnd(){
        yield return new WaitForSeconds(0.1f);
        attackColl.SetActive(false);
    }

    private void Update() {
        Jump();
        GroundChecker();

    }

    private void GroundChecker(){
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down,out hit,1f,LayerMask.GetMask("Ground"))){
            isJump = false;
            animator?.SetBool("isJump", isJump);
            Debug.Log("땅");
        }
        else{
            animator?.SetBool("isMove", false);

            isJump = true;
            animator?.SetBool("isJump", isJump);
        }
        Debug.DrawRay(transform.position + Vector3.up, Vector3.down * 1f, Color.yellow);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Ground"){
            
        }
    }
    private void OnCollisionStay(Collision other) {
        
    }

    private void OnCollisionExit(Collision other) {
    }
}

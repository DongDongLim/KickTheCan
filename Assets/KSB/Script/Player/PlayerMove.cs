using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DH
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField]
        PlayerScript owner = null;

        [SerializeField]
        private Transform cameraArm;

        [SerializeField]
        private Transform charactorBody;

        // 컴포넌트
        [SerializeField]
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

        // 그라운드 체크
        Vector3 rayStatePos;
        Vector2 boxCastSize;

        float maxGroundRayDistance;

        // 벽 체크
        bool isborder;

        float maxBorderRayDistance;

        float runnerOffset;


        private void Start()
        {
            cameraArm = transform.GetChild(0).transform;
        }

        public void Setting(Rigidbody r)
        { 
            UIMng.instance.jumpAction += Jump;
            owner = GetComponent<PlayerScript>();
            charactorBody = transform.GetChild(2).transform;
            maxGroundRayDistance = charactorBody.GetComponent<Collider>().bounds.size.y * 0.5f;
            maxBorderRayDistance = charactorBody.GetComponent<Collider>().bounds.size.x;
            rigid = r;
            if (owner.gameObject.layer == LayerMask.NameToLayer("Tagger"))
            {
                runnerOffset = 0;
            }
            else
            {
                runnerOffset = maxGroundRayDistance * 2;
            }
        }

        private void OnDestroy()
        {
            if (UIMng.instance != null)
                UIMng.instance.jumpAction -= Jump;
        }

        public void BorderChecker()
        {
            isborder = Physics.Raycast(charactorBody.position,charactorBody.forward,maxBorderRayDistance,LayerMask.GetMask("Wall"));
        }

        public void Move(Vector2 inputDirection)
        {
            moveInput = inputDirection;
            isMove = moveInput.magnitude != 0;

            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
            moveDir = moveDir.normalized;

            owner?.MoveAnim(isMove);

            if (!isMove)
                return;

            charactorBody.forward = moveDir;

            if (isborder)
                return;

            if (owner.isFreeze)
                return;

            rigid.MovePosition(transform.position + moveDir * Time.fixedDeltaTime * moveSpeed);
            //rigid.velocity = new Vector3(moveDir.x,rigid.velocity.y,moveDir.z);
        }
        public void LookAround(Vector2 inputDirection)
        {
            // 마우스 이동 값 검출
            Vector2 mouseDelta = inputDirection * rotateSpeed;
            // 카메라의 원래 각도를 오일러 각으로 저장
            Vector3 camAngle = cameraArm.rotation.eulerAngles;
            // 카메라의 피치 값 계산
            float x = camAngle.x + mouseDelta.y;

            // 카메라 피치 값을 위쪽으로 70도 아래쪽으로 25도 이상 움직이지 못하게 제한
            if (x < 180f)
            {
                x = Mathf.Clamp(x, -1f, 70f);
            }
            else
            {
                x = Mathf.Clamp(x, 335f, 361f);
            }

            mouseDelta = mouseDelta.normalized;

            // 카메라 암 회전 시키기
            cameraArm.rotation = Quaternion.Euler(x, camAngle.y - mouseDelta.x, camAngle.z);
        }

        public void Jump()
        {
            if (isJump)
                return;

            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

            owner?.MoveAnim(isMove);

            isJump = true;
            owner?.JumpAnim(isJump);
        }


        public void GroundChecker()
        {
            rayStatePos = new Vector3(transform.position.x, transform.position.y + maxGroundRayDistance, transform.position.z);
            boxCastSize = new Vector2(charactorBody.GetComponent<Collider>().bounds.size.x,charactorBody.GetComponent<Collider>().bounds.size.z);
            RaycastHit hit;
            if (Physics.SphereCast(rayStatePos + (Vector3.up * maxGroundRayDistance),boxCastSize.x * 0.5f,Vector3.down,out hit,maxGroundRayDistance + runnerOffset + 0.5f,LayerMask.GetMask("Ground","Object")))
            {
                isJump = false;
                owner?.JumpAnim(isJump);
            }
            else
            {
                owner?.MoveAnim(false);

                isJump = true;
                owner?.JumpAnim(isJump);
            }
        }

    }
}
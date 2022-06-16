using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DH
{
    public class PlayerMove : MonoBehaviour
    {
        Controller Aowner;
        Rigidbody rigid;

        private float hAxis = 0f;
        private float vAxis = 0f;
        private float moveSpeed = 5f;
        private float jumpPower = 5f;
        private Vector3 moveVec;    // 플레이어 움직이는 방향

        private bool isGrounded;                // 땅에 서있는지 체크하기 위한 bool값
        public float groundDistance = 0.5f;		// Ray를 쏴서 검사하는 거리

        public void Setting(Rigidbody r)
        {
            Aowner = GetComponent<Controller>();
            rigid = r;
        }

        public void Move()
        {
            hAxis = Input.GetAxis("Horizontal");
            vAxis = Input.GetAxis("Vertical");

            moveVec = new Vector3(hAxis, 0, vAxis).normalized;
            transform.LookAt(transform.position + moveVec);

            transform.position += moveVec * moveSpeed * Time.deltaTime;
        }

        public void Jump()
        {
            if (!isGrounded)
                return;

            if (!Input.GetButtonDown("Jump"))
                return;
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Ground")
            {
                isGrounded = true;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.tag == "Ground")
            {
                isGrounded = false;
            }
        }
    }
}
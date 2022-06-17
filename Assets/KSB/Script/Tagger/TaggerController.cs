using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace KSB
{
    public class TaggerController : Controller
    {
        // 공격

        // 최대 공격 가능 횟수
        private static int attackMaxCount = 5;

        // 현재 공격 가능 횟수
        private int attackCurCount;

        // 공격 횟수가 차는 시간
        private static float attackCool = 10f;
        private float attackCurCool;


        private void Awake() {
            attackCurCount = attackMaxCount;
        }
        public override void ControlUpdate()
        {
            move.GroundChecker();
            AttackCool();
        }

        public void Attack()
        {
            // 공격횟수가 없을 경우 공격 못함
            if (attackCurCount == 0)
                return;

            attackCurCount--;

            // 공격 행동
            attackColl?.SetActive(true);
            animator?.SetTrigger("isAttack");
            StartCoroutine("AttackEnd");
        }

        IEnumerator AttackEnd()
        {
            yield return new WaitForSeconds(0.1f);
            attackColl?.SetActive(false);
        }

        public void AttackCool(){
            Debug.Log("공격 가능 횟수 : " + attackCurCount);
            if (attackCurCount >= attackMaxCount)
                return;
            
            if (attackCurCool >= attackCool){
                attackCurCool = 0;
                attackCurCount++;
                Debug.Log(" 공격 횟수 1 증가 ");
                return;
            }
            Debug.Log("남은 쿨타임 : " + (attackCool - attackCurCool));
            attackCurCool += Time.deltaTime;
        }

        public override void ControllerAction()
        {
            Attack();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class TaggerController : Controller
    {
        // 공격

        // 최대 공격 가능 횟수
        private static int attackMaxCount = 5;

        // 현재 공격 가능 횟수
        private int attackCurCount;

        // 공격 횟수가 차는 시간
        private static float attackCool = 5f;
        private float attackCurCool;


        private void Awake() {
            CameraMng.instance.TaggerCamSetting();

            UIMng.instance.SetUI("Tagger");

            attackCurCount = attackMaxCount;
            UIMng.instance.SetAttackCount(attackCurCount);
        }
        public override void ControlUpdate()
        {
            move.BorderChecker();
            move.GroundChecker();
            AttackCool();
            Debug.Log("남은 공격 횟수 : " + attackCurCount);
        }

        public void AttackCool(){
            if (attackCurCount >= attackMaxCount)
                return;
            
            if (attackCurCool >= attackCool){
                attackCurCool = 0;
                attackCurCount++;
                UIMng.instance.SetAttackCount(attackCurCount);
                return;
            }
            attackCurCool += Time.deltaTime;
        }

        public void AttackComplete()
        {
            attackCurCount--;
            UIMng.instance.SetAttackCount(attackCurCount);
        }

        public override void ControllerAction()
        {
            if (0 < attackCurCount && GameManager.Instance.isAttack)
            {
                attackCurCount--;
                owner.photonView.RPC("Attack", RpcTarget.All);
            }
        }
    }
}

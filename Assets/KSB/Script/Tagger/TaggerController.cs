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
        private static float attackCool = 10f;
        private float attackCurCool;


        private void Awake() {
            attackCurCount = attackMaxCount;
            CameraMng.instance.TaggerCamSetting();
        }
        public override void ControlUpdate()
        {
            move.GroundChecker();
            AttackCool();
        }

        public void AttackCool(){
            //Debug.Log("공격 가능 횟수 : " + attackCurCount);
            if (attackCurCount >= attackMaxCount)
                return;
            
            if (attackCurCool >= attackCool){
                attackCurCool = 0;
                attackCurCount++;
                Debug.Log(" 공격 횟수 1 증가 ");
                return;
            }
            attackCurCool += Time.deltaTime;
        }

        public override void ControllerAction()
        {
            if (0 < attackCurCount)
            {
                attackCurCount--;
                owner.photonView.RPC("Attack", RpcTarget.All);
            }
        }
    }
}

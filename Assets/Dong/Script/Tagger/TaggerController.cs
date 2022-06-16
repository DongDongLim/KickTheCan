using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DH
{
    public class TaggerController : Controller
    {
        public override void ControlUpdate()
        {
            move.GroundChecker();
        }

        public void Attack()
        {
            attackColl?.SetActive(true);
            animator?.SetTrigger("isAttack");
            StartCoroutine("AttackEnd");
        }

        IEnumerator AttackEnd()
        {
            yield return new WaitForSeconds(0.1f);
            attackColl?.SetActive(false);
        }

        public override void ControllerAction()
        {
            Attack();
        }
    }
}

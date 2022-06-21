﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TaggerAttack : MonoBehaviour
{
    [SerializeField]
    GameObject attackColl;

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

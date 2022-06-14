using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rigid;
    private Animator anim;

    private float vInput = 0.0f;
    private float hInput = 0.0f;

    private float moveSpeed = 5.0f;
    private float rotateSpeed = 90.0f;

    public float health = 100.0f;

    public GameObject missile;
    public Transform shootPos;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = GameData.GetColor(photonView.Owner.GetPlayerNumber());
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        vInput = Input.GetAxis("Vertical");
        hInput = Input.GetAxis("Horizontal");

        transform.Translate(new Vector3(0, 0, vInput) * moveSpeed * Time.deltaTime, Space.Self);
        transform.Rotate(new Vector3(0, hInput, 0) * rotateSpeed * Time.deltaTime, Space.Self);

        anim.SetFloat("Speed", vInput);
        anim.SetFloat("Direction", hInput);

        if (Input.GetButtonDown("Fire1"))
        {
            //Fire();
            photonView.RPC("Fire", RpcTarget.All);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Heal();
            //photonView.RPC("Heal", RpcTarget.All);
        }
    }

    [PunRPC]
    private void Fire()
    {
        Instantiate(missile, shootPos.position, shootPos.rotation);
    }

    [PunRPC]
    private void Heal()
    {
        health += 10;
    }

    public void Hit(float damage)
    {
        health -= damage;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (float)stream.ReceiveNext();
        }
    }
}

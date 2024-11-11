using Mirror;
using UnityEngine;
using UnityEngine.Networking;



public class BallControl : NetworkBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Topla çarpýþan oyuncuya yetki devrediyoruz
            NetworkIdentity netIdentity = GetComponent<NetworkIdentity>();
            netIdentity.AssignClientAuthority(collision.gameObject.GetComponent<NetworkIdentity>().connectionToClient);
        }
    }
    [ClientRpc]
    void RpcMoveBall(Vector3 position, Quaternion rotation)
    {
        if (!isOwned) // Eðer bu istemcinin sahipliði yoksa güncellemeyi uygula
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }

    [Command]
    void CmdUpdateBallPosition(Vector3 position, Quaternion rotation)
    {
        RpcMoveBall(position, rotation); // Tüm istemcilerde güncelle
    }

    void Update()
    {
        if (isOwned) // Sadece yetkiye sahip olan oyuncu fizik simülasyonunu yapar
        {
            CmdUpdateBallPosition(transform.position, transform.rotation);
        }
    }
}
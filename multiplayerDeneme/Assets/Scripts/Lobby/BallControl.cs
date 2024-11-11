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
            // Topla �arp��an oyuncuya yetki devrediyoruz
            NetworkIdentity netIdentity = GetComponent<NetworkIdentity>();
            netIdentity.AssignClientAuthority(collision.gameObject.GetComponent<NetworkIdentity>().connectionToClient);
        }
    }
    [ClientRpc]
    void RpcMoveBall(Vector3 position, Quaternion rotation)
    {
        if (!isOwned) // E�er bu istemcinin sahipli�i yoksa g�ncellemeyi uygula
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }

    [Command]
    void CmdUpdateBallPosition(Vector3 position, Quaternion rotation)
    {
        RpcMoveBall(position, rotation); // T�m istemcilerde g�ncelle
    }

    void Update()
    {
        if (isOwned) // Sadece yetkiye sahip olan oyuncu fizik sim�lasyonunu yapar
        {
            CmdUpdateBallPosition(transform.position, transform.rotation);
        }
    }
}
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
    private void OnCollisionEnter2D(Collision2D collision) // 2D oldu�u i�in Collision2D olarak d�zelttik
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Topla �arp��an oyuncuya yetki devrediyoruz
            NetworkIdentity netIdentity = GetComponent<NetworkIdentity>();
            NetworkIdentity playerNetIdentity = collision.gameObject.GetComponent<NetworkIdentity>();

            if (netIdentity.isOwned == false)
            {
                // �nceki yetkiyi kald�r (yetki zaten varsa)
                if (netIdentity.connectionToClient != null)
                {
                    netIdentity.RemoveClientAuthority();
                }

                // Yeni oyuncuya yetki ver
                netIdentity.AssignClientAuthority(playerNetIdentity.connectionToClient);
                Debug.Log("Yetki " + playerNetIdentity.connectionToClient.connectionId + " oyuncusuna verildi.");
            }
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
            Debug.Log("Topun sahipli�i bende. Position: " + transform.position);
        }
    }
}
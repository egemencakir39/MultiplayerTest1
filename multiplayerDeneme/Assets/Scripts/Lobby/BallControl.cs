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
    private void OnCollisionEnter2D(Collision2D collision) // 2D olduðu için Collision2D olarak düzelttik
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Topla çarpýþan oyuncuya yetki devrediyoruz
            NetworkIdentity netIdentity = GetComponent<NetworkIdentity>();
            NetworkIdentity playerNetIdentity = collision.gameObject.GetComponent<NetworkIdentity>();

            if (netIdentity.isOwned == false)
            {
                // Önceki yetkiyi kaldýr (yetki zaten varsa)
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
            Debug.Log("Topun sahipliði bende. Position: " + transform.position);
        }
    }
}
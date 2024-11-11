using Mirror;
using UnityEngine;
using UnityEngine.Networking;



public class BallControl : NetworkBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            NetworkIdentity netIdentity = GetComponent<NetworkIdentity>();
            NetworkIdentity playerNetIdentity = collision.gameObject.GetComponent<NetworkIdentity>();

            // Yetkiyi oyuncuya ver
            if (netIdentity.connectionToClient != playerNetIdentity.connectionToClient)
            {
                if (netIdentity.connectionToClient != null)
                {
                    netIdentity.RemoveClientAuthority();
                }
                netIdentity.AssignClientAuthority(playerNetIdentity.connectionToClient);
            }

            // Çarpýþma yönünde kuvvet uygula
            Vector2 forceDirection = collision.relativeVelocity.normalized * 5f; // Kuvvet miktarýný ayarla
            rb.AddForce(forceDirection, ForceMode2D.Impulse);

            // Yetkiyi geri al
            Invoke(nameof(RemoveAuthority), 0.1f); // Küçük bir gecikme ile yetkiyi kaldýr
        }
    }

    [Server]
    private void RemoveAuthority()
    {
        NetworkIdentity netIdentity = GetComponent<NetworkIdentity>();
        if (netIdentity.connectionToClient != null)
        {
            netIdentity.RemoveClientAuthority();
        }
    }
}
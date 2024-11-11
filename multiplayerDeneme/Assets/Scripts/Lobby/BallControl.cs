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

            // �arp��ma y�n�nde kuvvet uygula
            Vector2 forceDirection = collision.relativeVelocity.normalized * 5f; // Kuvvet miktar�n� ayarla
            rb.AddForce(forceDirection, ForceMode2D.Impulse);

            // Yetkiyi geri al
            Invoke(nameof(RemoveAuthority), 0.1f); // K���k bir gecikme ile yetkiyi kald�r
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
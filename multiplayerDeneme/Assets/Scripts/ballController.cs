using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ballController : NetworkBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Eðer client authority'ye sahipsek, topu sadece client'ta hareket ettiriyoruz
        if (isOwned)
        {
            HandleBallMovement();
        }
    }

    private void HandleBallMovement()
    {
        // Basit bir kontrol mekanizmasý; topu ileriye doðru hareket ettiriyoruz
        float moveSpeed = 10f;
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y); // Yalnýzca X ekseninde hareket ettiriyoruz
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Sadece sunucu tarafýnda çalýþtýrmak için kontrol ediyoruz
        if (isServer && collision.gameObject.CompareTag("Player"))
        {
            NetworkIdentity playerIdentity = collision.gameObject.GetComponent<NetworkIdentity>();
            if (playerIdentity != null)
            {
                // Topun authority'si var mý? Varsa, önce authority'yi kaldýrýyoruz
                NetworkIdentity ballIdentity = GetComponent<NetworkIdentity>();

                if (ballIdentity.isOwned)
                {
                    ballIdentity.RemoveClientAuthority();
                }

                // Yetki devri iþlemi
                AssignAuthority(playerIdentity.connectionToClient);
            }
        }
    }

    [Server]
    private void AssignAuthority(NetworkConnectionToClient conn)
    {
        NetworkIdentity ballIdentity = GetComponent<NetworkIdentity>();

        // Yeni client'a yetki veriyoruz
        ballIdentity.AssignClientAuthority(conn);
    }
}
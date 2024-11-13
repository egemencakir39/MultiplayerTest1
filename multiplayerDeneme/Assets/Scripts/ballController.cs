using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ballController : NetworkBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Sadece sunucu taraf�nda �al��t�rmak i�in kontrol ediyoruz
        if (isServer && collision.gameObject.CompareTag("Player"))
        {
            NetworkIdentity playerIdentity = collision.gameObject.GetComponent<NetworkIdentity>();
            if (playerIdentity != null)
            {
                // Yetki devri i�lemi i�in kontrol ediyoruz
                if (!GetComponent<NetworkIdentity>().isOwned)
                {
                    // Topun daha �nce atanm�� bir yetkisi varsa kald�r�yoruz
                    AssignAuthority(playerIdentity.connectionToClient);
                }
            }
        }
    }

    [Server]
    private void AssignAuthority(NetworkConnectionToClient conn)
    {
        NetworkIdentity ballIdentity = GetComponent<NetworkIdentity>();

        // Topun daha �nce atanm�� bir yetkisi varsa, �ncelikle kald�r�yoruz
        if (ballIdentity.isOwned)
        {
            ballIdentity.RemoveClientAuthority();
        }

        // Yeni client'a yetki veriyoruz
        ballIdentity.AssignClientAuthority(conn);
    }
}
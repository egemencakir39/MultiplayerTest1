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
                // Ba�lant�y� do�ru t�rdeki ba�lant� nesnesine d�n��t�rerek yetkiyi devrediyoruz
                AssignAuthority(playerIdentity.connectionToClient);
            }
        }
    }

    [Server]
    private void AssignAuthority(NetworkConnectionToClient conn)
    {
        // Ball'�n NetworkIdentity bile�enini al�yoruz
        NetworkIdentity ballIdentity = GetComponent<NetworkIdentity>();

        // Topun daha �nce atanm�� bir yetkisi varsa kald�r�yoruz
        if (ballIdentity.isOwned)
        {
            ballIdentity.RemoveClientAuthority();
        }

        // Yeni client'a yetki veriyoruz
        ballIdentity.AssignClientAuthority(conn);
    }
}
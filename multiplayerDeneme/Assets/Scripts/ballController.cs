using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ballController : NetworkBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Sadece sunucu tarafýnda çalýþtýrmak için kontrol ediyoruz
        if (isServer && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("asd");
            NetworkIdentity playerIdentity = collision.gameObject.GetComponent<NetworkIdentity>();
            if (playerIdentity != null)
            {
                // Yetkiyi devretmeden önce mevcut yetkiyi kaldýrýyoruz
                AssignAuthority(playerIdentity.connectionToClient);
            }
        }
    }

    [Server]
    private void AssignAuthority(NetworkConnectionToClient conn)
    {
        NetworkIdentity ballIdentity = GetComponent<NetworkIdentity>();

        // Topun daha önce atanmýþ bir yetkisi varsa, öncelikle kaldýrýyoruz
        if (ballIdentity.isOwned)
        {
            ballIdentity.RemoveClientAuthority();
        }

        // Yeni client'a yetki veriyoruz
        ballIdentity.AssignClientAuthority(conn);
    }
}
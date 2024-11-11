using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BallControl : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isLocalPlayer) // Yalnýzca yerel oyuncunun inputlarýný al
        {
            // Hareketi yalnýzca yerel oyuncu inputu ile sunucuya gönder
            Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * moveSpeed * Time.deltaTime;
            CmdMoveBall(movement); // Hareketi sunucuya gönder
        }
    }

    // Komut kullanarak topu hareket ettir
    [Command]
    private void CmdMoveBall(Vector2 movement)
    {
        // Sunucuda topu hareket ettir
        rb.MovePosition(rb.position + movement);
        RpcUpdateBallPosition(rb.position); // Pozisyonu istemcilere güncelle
    }

    // RPC ile topun yeni pozisyonunu tüm istemcilere gönder
    [ClientRpc]
    private void RpcUpdateBallPosition(Vector2 newPosition)
    {
        // Topun pozisyonunu istemcilerde güncelle
        if (!isServer) // Sunucuda bu fonksiyon çalýþmasýn
        {
            rb.position = newPosition;
        }
    }
}

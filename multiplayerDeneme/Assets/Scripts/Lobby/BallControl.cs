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
        if (isLocalPlayer) // Yaln�zca yerel oyuncunun inputlar�n� al
        {
            // Hareketi yaln�zca yerel oyuncu inputu ile sunucuya g�nder
            Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * moveSpeed * Time.deltaTime;
            CmdMoveBall(movement); // Hareketi sunucuya g�nder
        }
    }

    // Komut kullanarak topu hareket ettir
    [Command]
    private void CmdMoveBall(Vector2 movement)
    {
        // Sunucuda topu hareket ettir
        rb.MovePosition(rb.position + movement);
        RpcUpdateBallPosition(rb.position); // Pozisyonu istemcilere g�ncelle
    }

    // RPC ile topun yeni pozisyonunu t�m istemcilere g�nder
    [ClientRpc]
    private void RpcUpdateBallPosition(Vector2 newPosition)
    {
        // Topun pozisyonunu istemcilerde g�ncelle
        if (!isServer) // Sunucuda bu fonksiyon �al��mas�n
        {
            rb.position = newPosition;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BallControl : NetworkBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 5f; // Topun hareket h�z�n� ayarlayabiliriz.

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isServer) // Sadece sunucu topu hareket ettirir
        {
            // �arp��ma sonucu topun hareket etmesi
            // Bu k�sm� oyuncular�n topa fiziksel etkile�ime girmesiyle sa�layaca��z
            // Burada fiziksel etkile�im i�in ek kontrol gerekebilir
        }
    }

    // Sunucuya ba�l� olarak topun yeni pozisyonu t�m istemcilere iletilir
    [ClientRpc]
    private void RpcUpdateBallPosition(Vector2 newPosition)
    {
        // Topun konumunu istemcilerde g�ncelle
        if (!isServer)
        {
            rb.position = newPosition;
        }
    }

    // Topa �arpan oyuncudan gelen hareketi i�leyelim
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Burada �arp��ma ile ilgili hareketi fiziksel olarak i�leyebilirsiniz
        // Herhangi bir oyuncu topa �arpt���nda, topu hareket ettirebiliriz
        if (collision.gameObject.CompareTag("Player")) // Oyuncu ile �arp��ma
        {
            // Topun hareket etmesini sa�la, buraya itme kuvveti uygulayabilirsiniz
            Vector2 pushDirection = collision.relativeVelocity.normalized; // �arpma y�n�
            rb.AddForce(pushDirection * moveSpeed, ForceMode2D.Impulse); // Topu itme

            // Yeni pozisyonu t�m istemcilerle g�ncelle
            RpcUpdateBallPosition(rb.position);
        }
    }
}

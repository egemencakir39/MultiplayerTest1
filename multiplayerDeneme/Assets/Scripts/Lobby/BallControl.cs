using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BallControl : NetworkBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 5f; // Topun hareket hýzýný ayarlayabiliriz.

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isServer) // Sadece sunucu topu hareket ettirir
        {
            // Çarpýþma sonucu topun hareket etmesi
            // Bu kýsmý oyuncularýn topa fiziksel etkileþime girmesiyle saðlayacaðýz
            // Burada fiziksel etkileþim için ek kontrol gerekebilir
        }
    }

    // Sunucuya baðlý olarak topun yeni pozisyonu tüm istemcilere iletilir
    [ClientRpc]
    private void RpcUpdateBallPosition(Vector2 newPosition)
    {
        // Topun konumunu istemcilerde güncelle
        if (!isServer)
        {
            rb.position = newPosition;
        }
    }

    // Topa çarpan oyuncudan gelen hareketi iþleyelim
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Burada çarpýþma ile ilgili hareketi fiziksel olarak iþleyebilirsiniz
        // Herhangi bir oyuncu topa çarptýðýnda, topu hareket ettirebiliriz
        if (collision.gameObject.CompareTag("Player")) // Oyuncu ile çarpýþma
        {
            // Topun hareket etmesini saðla, buraya itme kuvveti uygulayabilirsiniz
            Vector2 pushDirection = collision.relativeVelocity.normalized; // Çarpma yönü
            rb.AddForce(pushDirection * moveSpeed, ForceMode2D.Impulse); // Topu itme

            // Yeni pozisyonu tüm istemcilerle güncelle
            RpcUpdateBallPosition(rb.position);
        }
    }
}

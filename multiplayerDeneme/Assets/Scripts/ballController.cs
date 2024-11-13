using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetworkIdentity))]
public class ballController : NetworkBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    [ServerCallback]
    void FixedUpdate()
    {
        // Topun sunucu taraf�ndan hareket ettirilmesi ve fiziksel etkile�imlerinin kontrol edilmesi
    }

    [ServerCallback]
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �arpma olay�n� y�net
            Vector2 force = collision.relativeVelocity * rb.mass;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}

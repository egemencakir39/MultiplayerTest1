using Mirror;
using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(Rigidbody2D))]
public class BallControl : NetworkBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    [ServerCallback]
    void FixedUpdate()
    {
        // Top hareketi sunucu tarafýndan kontrol edilir
    }

    [ServerCallback]
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Çarpma olayýný yönet
            Vector2 force = collision.relativeVelocity * rb.mass;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}

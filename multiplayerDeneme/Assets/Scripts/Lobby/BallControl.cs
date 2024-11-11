using Mirror;
using UnityEngine;
using UnityEngine.Networking;


public class BallControl : NetworkBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Top hareketi ve fiziksel etkile�imler sunucu taraf�ndan kontrol edilir
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

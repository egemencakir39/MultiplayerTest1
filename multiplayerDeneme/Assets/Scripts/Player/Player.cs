using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [Header("Variables")]
    [SerializeField] private float force = 5f;
    public float currentSpeed;

    [Header("Physics")]
    private Vector2 movemenet;
    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(movemenet);
        currentSpeed = rb.velocity.magnitude;
        rb.velocity *= 0.95f;
    }
    private void Update()
    {
        if (isLocalPlayer)
        {
            Movement();
        }
    }
    private void Movement()
    {
        float Mhorizontal = Input.GetAxisRaw("Horizontal");
        float Mvertical = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(Mhorizontal, Mvertical);
        

        if (input.magnitude > 1)
        {
          input.Normalize();
        }
         movemenet = input * force;
    }
}

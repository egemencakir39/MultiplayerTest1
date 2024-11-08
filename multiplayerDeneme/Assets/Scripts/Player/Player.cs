using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class Player : NetworkBehaviour
{
    [Header("Variables")]
    [SerializeField] private float force = 5f;
    public float currentSpeed;
    public GameObject PlayerModel;

    [Header("Physics")]
    [SyncVar]
    private Vector2 movement;
    Rigidbody2D rb;
    private void Start()
    {
        PlayerModel.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            rb.AddForce(movement);
            currentSpeed = rb.velocity.magnitude;
            rb.velocity *= 0.95f;
        }

    }
    private void Update()
    {
        
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (PlayerModel.activeSelf == false)
            {
                PlayerModel.SetActive(true);
                SetPosition();
            }
            if (isLocalPlayer)
            {
                Movement();
                if (NetworkClient.ready) // Ýstemcinin hazýr olup olmadýðýný kontrol et
                {
                    CmdMove(movement);
                }
                else
                {
                    NetworkClient.Ready(); // Ýstemciyi hazýr duruma geçir
                }
            }
        }
    }

    public void SetPosition()
    {
        transform.position = new Vector2(Random.Range(0, 5), Random.Range(0, 3));
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
         movement = input * force;
    }
    [Command(requiresAuthority = false)]
    private void CmdMove(Vector2 movementDirection)
    {
        movement = movementDirection;
        RpcMove(movementDirection);
    }
    [ClientRpc]
    private void RpcMove(Vector2 movementDirection)
    {
        if (!isLocalPlayer) // Yerel oyuncuda tekrar iþlem yapýlmasýný önlüyoruz
        {
            movement = movementDirection;
        }
    }

}

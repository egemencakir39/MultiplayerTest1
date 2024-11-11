using Mirror;
using UnityEngine;
using UnityEngine.Networking;



public class BallControl : NetworkBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Çarpan oyuncuya yetki devret
            NetworkIdentity netIdentity = GetComponent<NetworkIdentity>();
            NetworkIdentity playerNetIdentity = collision.gameObject.GetComponent<NetworkIdentity>();

            if (netIdentity.connectionToClient != playerNetIdentity.connectionToClient)
            {
                if (netIdentity.connectionToClient != null)
                {
                    netIdentity.RemoveClientAuthority();
                }
                netIdentity.AssignClientAuthority(playerNetIdentity.connectionToClient);
            }
        }
    }

    [Command]
    public void CmdApplyForce(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
        RpcUpdateBallPosition(transform.position, rb.velocity);
    }

    [ClientRpc]
    private void RpcUpdateBallPosition(Vector3 position, Vector2 velocity)
    {
        transform.position = position;
        rb.velocity = velocity;
    }
}
using Mirror;
using UnityEngine;
using UnityEngine.Networking;


public class BallControl : NetworkBehaviour
{
    private Rigidbody2D rb;

    public override void OnStartServer()
    {
        base.OnStartServer();

        rb.simulated = true;
    }
}

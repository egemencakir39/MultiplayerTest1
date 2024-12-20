using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;


public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectControl GamePlayerPrefab;
    [SerializeField] private GameObject ballPrefab;
 

  
    public List<PlayerObjectControl> GamePlayers { get; } = new List<PlayerObjectControl>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectControl GamePlayerInstance = Instantiate(GamePlayerPrefab);
            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerID = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.LobbyId, GamePlayers.Count);
            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }
   
    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
    }
    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName == "GameScene")
        {
           
            GameObject ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(ball);
        }
    }
}

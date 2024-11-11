using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;


public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectControl GamePlayerPrefab;
    [SerializeField] private GameObject Ball;
    public string gameSceneName = "GameScene";
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
    private void Start()
    {
       
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Eðer oyun sahnesindeysek ve sunucuysak topu spawn et
        if (scene.name == gameSceneName)
        {
            SpawnBall();
        }
    }
    private void SpawnBall()
    {
        // Topu merkezde oluþtur ve að üzerinden spawn et
        GameObject ball = Instantiate(Ball, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(ball);
    }
    private void OnDestroy()
    {
        // Olay dinleyicisini temizle
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;

public class PlayerObjectControl : NetworkBehaviour
{
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerID;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

    private CustomNetworkManager manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.Ready = newValue;
        }
        if (isClient)
        {
            LobbyControler.Instance.UpdatePlayerList();
        }
    }
    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }

    public void ChangeReady()
    {
        if (isOwned)
        {
            CmdSetPlayerReady();
        }
    }

    public override void OnStartAuthority()
    {
        PlayerName = SteamFriends.GetFriendPersonaName().ToString();
        Debug.Log("Steam arkadaþ ismi alýndý " + PlayerName);
        CmdSetPlayerName(PlayerName);  
        gameObject.name = "LocalGamePlayer";
        LobbyControler.Instance.FindLocalPlayer();
        LobbyControler.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyControler.Instance.UpdateLobbyName();
        LobbyControler.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyControler.Instance.UpdatePlayerList();
    }
    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        Debug.Log("CmdSetPlayerName çalýþýyor: " + playerName);
        PlayerName = playerName; 
    }

    public void PlayerNameUpdate(string oldName, string newName)
    {
        if (isServer)
        {
            PlayerName = newName;
        }
        if (isClient)
        {
            Debug.Log("Client tarafýnda güncellenen isim: " + newName);
            LobbyControler.Instance.UpdatePlayerList();
        }
    }

    public void CanStartGame(string sceneName)
    {
        if (isOwned)
        {
            CmdCanStartGame(sceneName);
        }
    }
    [Command]
    public void CmdCanStartGame(string sceneName)
    {
        manager.StartGame(sceneName);
    }

   
}

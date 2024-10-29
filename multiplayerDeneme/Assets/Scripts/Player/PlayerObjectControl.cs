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

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(PlayerName);
        PlayerName = SteamFriends.GetFriendPersonaName().ToString();
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
       PlayerNameUpdate(PlayerName, playerName);
    }
    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if (isServer)
        {
           this.PlayerName = NewValue;
        }
        if (isClient)
        {
            LobbyControler.Instance.UpdatePlayerList();
        }
    }
}
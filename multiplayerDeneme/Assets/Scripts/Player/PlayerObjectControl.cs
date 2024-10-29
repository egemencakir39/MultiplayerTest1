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
        PlayerName = SteamFriends.GetFriendPersonaName().ToString();
        Debug.Log("Steam arkadaþ ismi alýndý: " + PlayerName);
       
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
        Debug.Log("PlayerNameUpdate çalýþýyor. Eski Deðer: " + oldName + ", Yeni Deðer: " + newName);
       
        if (isServer)
        {
            PlayerName = newName;
        }
        if (isClientOnly)
        {
            LobbyControler.Instance.UpdatePlayerList();
           
        }
    }
}

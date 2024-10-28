using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> LobbyEnter;

    [Header("Variables")]
    public ulong LobbyId;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager manager;

    

    private void Start()
    {
        if (!SteamManager.Initialized) { return; }

        if (Instance == null) { Instance = this; }
        
        manager = GetComponent<CustomNetworkManager>();
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callBack)
    {
       if (callBack.m_eResult != EResult.k_EResultOK) { return; }

        
        manager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callBack.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callBack.m_ulSteamIDLobby),"name",SteamFriends.GetPersonaName().ToString()+"'S Lobby");
    }
    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Katýldý");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
       
        LobbyId = callback.m_ulSteamIDLobby;

        if (NetworkServer.active) { return; }

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        manager.StartClient();

    }

}

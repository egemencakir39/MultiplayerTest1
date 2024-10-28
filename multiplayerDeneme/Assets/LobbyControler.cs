using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SocialPlatforms;

public class LobbyControler : MonoBehaviour
{
    public static LobbyControler Instance;

    public Text LobbyNameText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    public ulong LobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListIems = new List<PlayerListItem>();
    public PlayerObjectControl LocalplayerController;

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

    private void Awake()
    {
        if(Instance == null) { Instance = this; }
    }

    public void UpdateLobbyName()
    {
        LobbyID = Manager.GetComponent<SteamLobby>().LobbyId;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(LobbyID), "name");
    }

    public void UpdatePlayerList()
    {
       
        if (!PlayerItemCreated) { CreateHostPlayerItem(); }
        if(PlayerListIems.Count < Manager.GamePlayers.Count) { CreateClientPlayerItem(); }
        if (PlayerListIems.Count > Manager.GamePlayers.Count) { RemovePlayerItem(); }
        if (PlayerListIems.Count == Manager.GamePlayers.Count) { UpdatePlayerItem(); }
        
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalplayerController = LocalPlayerObject.GetComponent<PlayerObjectControl>();
    }
    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectControl player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();
            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.SetPlayersValues();

            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListIems.Add(NewPlayerItemScript);

        }
        PlayerItemCreated = true;
    }
    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectControl player in Manager.GamePlayers)
        {
            if (!PlayerListIems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();
                
                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.SetPlayersValues();

                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;

                PlayerListIems.Add(NewPlayerItemScript);

            }
        }
    }
    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectControl player in Manager.GamePlayers)
        {
            foreach (PlayerListItem PlayerListItemScript in PlayerListIems)
            {
                if (PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.SetPlayersValues();
                }


            }
        }

    }
    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();
        foreach (PlayerListItem PlayerListItem in PlayerListIems)
        {
            if (!Manager.GamePlayers.Any(b=>b.ConnectionID == PlayerListItem.ConnectionID))
            {
                playerListItemToRemove.Add(PlayerListItem);
            }
        }
        if (playerListItemToRemove.Count>0)
        {
            foreach (PlayerListItem playerlistItemToRemove in playerListItemToRemove)
            {
                GameObject ObjectToRemove = playerlistItemToRemove.gameObject;
                PlayerListIems.Remove(playerlistItemToRemove);
                Destroy(ObjectToRemove);
                ObjectToRemove = null;
            }
        }

    }
}

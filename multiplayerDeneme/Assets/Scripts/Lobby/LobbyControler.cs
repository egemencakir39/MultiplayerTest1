using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using System.Linq;

public class LobbyControler : MonoBehaviour
{
    public static LobbyControler Instance;
    // UI Elements
    public Text LobbyNameText;

    // Player Data
    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    // Other Data
    public ulong LobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectControl LocalplayerController;

    // Ready State
    public Button StartGameButton;
    public Text ReadyButtonText;

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
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ReadyPlayer()
    {
        LocalplayerController.ChangeReady();
    }

    public void UpdateButton()
    {
        if (LocalplayerController.Ready)
        {
            ReadyButtonText.text = "Unready";
        }
        else
        {
            ReadyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady()
    {
        bool AllReady = false;

        foreach (PlayerObjectControl player in Manager.GamePlayers)
        {
            if (player.Ready)
            {
                AllReady = true;
            }
            else
            {
                AllReady = false;
                break;
            }
        }
        if (AllReady)
        {
            if (LocalplayerController.PlayerID == 1)
            {
                StartGameButton.interactable = true;
            }
            else
            {
                StartGameButton.interactable = false;
            }
        }
        else
        {
            StartGameButton.interactable = false;
        }
    }
    public void UpdateLobbyName()
    {
        LobbyID = Manager.GetComponent<SteamLobby>().LobbyId;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(LobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated)
        {
            CreateHostPlayerItem();
        }
        if (PlayerListItems.Count > Manager.GamePlayers.Count)
        {
            RemovePlayerItem();
        }
        if (PlayerListItems.Count == Manager.GamePlayers.Count)
        {
            UpdatePlayerItem();
        }
        if (PlayerListItems.Count < Manager.GamePlayers.Count)
        {
            CreateClientPlayerItem();
        }
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
            NewPlayerItemScript.Ready = player.Ready;
            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectControl player in Manager.GamePlayers)
        {
            if (!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();
                NewPlayerItemScript.PlayerName = player.PlayerName;
                Debug.Log(player.PlayerName);
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.SetPlayersValues();
                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;
                NewPlayerItemScript.Ready = player.Ready;
                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectControl player in Manager.GamePlayers)
        {
            foreach (PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if (PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.Ready = player.Ready;
                    PlayerListItemScript.SetPlayersValues();
                    if (player == LocalplayerController)
                    {
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();
        foreach (PlayerListItem PlayerListItem in PlayerListItems)
        {
            if (!Manager.GamePlayers.Any(b => b.ConnectionID == PlayerListItem.ConnectionID))
            {
                playerListItemToRemove.Add(PlayerListItem);
            }
        }
        if (playerListItemToRemove.Count > 0)
        {
            foreach (PlayerListItem playerlistItemToRemove in playerListItemToRemove)
            {
                GameObject ObjectToRemove = playerlistItemToRemove.gameObject;
                PlayerListItems.Remove(playerlistItemToRemove);
                Destroy(ObjectToRemove);
                ObjectToRemove = null;
            }
        }
    }
    public void StartGame(string sceneName)
    {
        LocalplayerController.CanStartGame(sceneName);
    }
}


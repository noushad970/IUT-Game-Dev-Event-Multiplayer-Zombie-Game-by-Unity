using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviourPunCallbacks
{
    public GameObject lobbyPanel;
    public GameObject roomPanel;

    public TMP_Text roomNameText;
    public TMP_Text playerCountText;

    public Transform playerParent;
    public GameObject playerItemPrefab;

    public Button startButton;
    public Button leaveButton;

    void Start()
    {
        leaveButton.onClick.AddListener(LeaveRoom);
        startButton.onClick.AddListener(StartGame);
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        roomNameText.text = "Room name: "+PhotonNetwork.CurrentRoom.Name;

        RefreshPlayers();

        startButton.gameObject.SetActive(
            PhotonNetwork.IsMasterClient);
    } 

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RefreshPlayers();
    }

    void RefreshPlayers()
    {
        foreach (Transform child in playerParent)
            Destroy(child.gameObject);

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject obj =
                Instantiate(playerItemPrefab, playerParent);

            TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();

            string playerName = string.IsNullOrEmpty(p.NickName)
                ? "Unknown"
                : p.NickName;

            if (p.IsMasterClient)
                txt.text = playerName + " (Host)";
            else
                txt.text = playerName;
        }

        playerCountText.text =
            PhotonNetwork.CurrentRoom.PlayerCount +
            " / " +
            PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.JoinLobby();

        Debug.Log("Connected to Photon");
    }
    void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        Debug.Log("Starting Multiplayer Game...");

        PhotonNetwork.LoadLevel("Multiplayer");
    }
}
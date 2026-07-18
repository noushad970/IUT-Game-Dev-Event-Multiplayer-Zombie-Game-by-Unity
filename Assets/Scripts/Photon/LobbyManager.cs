using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Create Room")]
    public TMP_InputField roomNameInput;
    public Button createRoomButton;

    public byte maxPlayers = 4;

    [Header("Room List")]
    public Transform roomListParent;
    public GameObject roomItemPrefab;

    [Header("Panels")]
    public GameObject mainLobbyPanel;
    public GameObject roomListPanel;

    [Header("Buttons")]
    public Button browseRoomsButton;
    public Button backButton;
    [Header("Debug")]
    public TMP_Text debugText;
    private Dictionary<string, RoomInfo> cachedRooms =
        new Dictionary<string, RoomInfo>();

    private Dictionary<string, RoomItem> roomItems =
        new Dictionary<string, RoomItem>();

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        createRoomButton.onClick.AddListener(CreateRoom);

        browseRoomsButton.onClick.AddListener(OpenRoomList);
        backButton.onClick.AddListener(CloseRoomList);

        roomListPanel.SetActive(false);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    void SetDebug(string message)
    {
        Debug.Log(message);

        if (debugText != null)
            debugText.text = message;
    }
    //--------------------------------------------------

    public void CreateRoom()
    {
        string roomName = roomNameInput.text.Trim();

        if (string.IsNullOrEmpty(roomName))
        {
            Debug.Log("Room Name Empty");
            SetDebug("Room Name Empty");
            return;
        }

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = maxPlayers
        };

        PhotonNetwork.CreateRoom(roomName, options);

        Debug.Log("Creating Room : " + roomName);
    }
    public void OpenRoomList()
    {
        mainLobbyPanel.SetActive(false);
        roomListPanel.SetActive(true);

        Debug.Log("Opened Room List");
    }

    public void CloseRoomList()
    {
        roomListPanel.SetActive(false);
        mainLobbyPanel.SetActive(true);

        Debug.Log("Closed Room List");
    }
    //--------------------------------------------------

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created");
    }

    public override void OnCreateRoomFailed(short code, string message)
    {
        Debug.Log("Create Failed : " + message);
    }

    //--------------------------------------------------

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined : " + PhotonNetwork.CurrentRoom.Name);

        // SceneManager.LoadScene("Game");
    }

    //--------------------------------------------------

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                if (roomItems.ContainsKey(room.Name))
                {
                    Destroy(roomItems[room.Name].gameObject);

                    roomItems.Remove(room.Name);
                }

                cachedRooms.Remove(room.Name);

                continue;
            }

            cachedRooms[room.Name] = room;

            if (roomItems.ContainsKey(room.Name))
            {
                roomItems[room.Name].Initialize(room);
            }
            else
            {
                GameObject obj =
                    Instantiate(roomItemPrefab, roomListParent);

                RoomItem item = obj.GetComponent<RoomItem>();

                item.Initialize(room);

                roomItems.Add(room.Name, item);
            }
        }
    }
}
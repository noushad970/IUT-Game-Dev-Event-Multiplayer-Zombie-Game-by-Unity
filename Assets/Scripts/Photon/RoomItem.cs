using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomName;

    public TMP_Text playerCount;

    public Button joinButton;

    RoomInfo room;

    public void Initialize(RoomInfo info)
    {
        room = info;

        roomName.text = info.Name;

        playerCount.text =
            info.PlayerCount + "/" + info.MaxPlayers;

        joinButton.onClick.RemoveAllListeners();

        joinButton.onClick.AddListener(JoinRoom);
    }

    void JoinRoom()
    {
        PhotonNetwork.JoinRoom(room.Name);

        Debug.Log("Joining : " + room.Name);
    }
}
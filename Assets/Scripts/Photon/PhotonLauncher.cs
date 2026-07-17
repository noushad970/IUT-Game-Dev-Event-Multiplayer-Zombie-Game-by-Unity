using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public static PhotonLauncher Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to Photon...");

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "1.0";

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master Server");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }
}
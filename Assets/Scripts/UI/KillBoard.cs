using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KillBoard : MonoBehaviourPunCallbacks
{
    public static KillBoard Instance;

    public TMP_Text boardText;
    public TMP_Text mykillText;
    public TMP_Text totalCoinEarnText;

    public static int myKills = 0;
    public static int totalCoinsEarned = 0;

    public Button mainMenuButton;

    private bool leavingRoom = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainMenuButton.onClick.AddListener(GotoMainMenu);
    }

    private void Update()
    {
        mykillText.text = "My Total Zombie Kills : " + myKills;
        totalCoinEarnText.text = "Total Coins Bonus : " + totalCoinsEarned;
    }

    //---------------------------------------------------

    void GotoMainMenu()
    {
        if (leavingRoom)
            return;

        leavingRoom = true;

        mainMenuButton.interactable = false;

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Leaving Room...");
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            DisconnectPhoton();
        }
    }

    //---------------------------------------------------

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");

        DisconnectPhoton();
    }

    //---------------------------------------------------

    void DisconnectPhoton()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Disconnecting Photon...");
            PhotonNetwork.Disconnect();
        }
        else
        {
            LoadMainMenu();
        }
    }

    //---------------------------------------------------

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected : " + cause);

        LoadMainMenu();
    }

    //---------------------------------------------------

    void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    //---------------------------------------------------

    public void Refresh()
    {
        boardText.text = "";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject obj = FindPlayer(player);

            if (obj == null)
                continue;

            PlayerStatsMulti stats = obj.GetComponent<PlayerStatsMulti>();

            boardText.text += player.NickName + ": " + stats.zombieKills + "\n";
        }
    }

    //---------------------------------------------------

    GameObject FindPlayer(Player player)
    {
        PlayerStatsMulti[] all =
            FindObjectsByType<PlayerStatsMulti>(FindObjectsSortMode.None);

        foreach (PlayerStatsMulti p in all)
        {
            if (p.photonView.Owner == player)
                return p.gameObject;
        }

        return null;
    }
}
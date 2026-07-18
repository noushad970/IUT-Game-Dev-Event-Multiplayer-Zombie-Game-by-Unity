using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviourPunCallbacks
{
    public static GameTimer Instance;

    [Header("UI")]
    public TMP_Text timerText;

    [Header("Match")]
    public float matchDuration = 300f; // 5 minutes

    private float currentTime;
    private bool matchEnded;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_StartTimer),
                RpcTarget.AllBuffered,
                matchDuration);
        }
    }

    [PunRPC]
    void RPC_StartTimer(float time)
    {
        currentTime = time;
        matchEnded = false;
    }

    private void Update()
    {
        if (matchEnded)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime < 0)
            currentTime = 0;

        int minute = Mathf.FloorToInt(currentTime / 60);
        int second = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minute:00}:{second:00}";

        if (currentTime <= 0)
        {
            matchEnded = true;

            photonView.RPC(nameof(RPC_EndGame), RpcTarget.All);
        }
    }

    //-------------------------------------------------------

    [PunRPC]
    void RPC_EndGame()
    {
        matchEnded = true;

        Debug.Log("MATCH FINISHED");

        PlayerStatsMulti stats = PlayerStatsMulti.LocalPlayer;

        if (stats != null)
        {
            stats.ShowDeathUI();
        }

        DisableLocalPlayer();
    }

    //-------------------------------------------------------

    void DisableLocalPlayer()
    {
        PhotonView[] players =
            FindObjectsByType<PhotonView>(FindObjectsSortMode.None);

        foreach (PhotonView pv in players)
        {
            if (!pv.IsMine)
                continue;


            

            break;
        }
    }
}
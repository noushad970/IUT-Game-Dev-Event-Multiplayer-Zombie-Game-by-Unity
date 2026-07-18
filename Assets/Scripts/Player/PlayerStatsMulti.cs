using Photon.Pun;
using UnityEngine;
using System.Collections;

public class PlayerStatsMulti : MonoBehaviourPun
{
    public static PlayerStatsMulti LocalPlayer;

    [Header("Stats")]
    public int zombieKills;
    public int coinsEarnedThisMatch;

    [Header("Rewards")]
    public int zombieReward = 10;
    public int bossReward = 500;

    [Header("Death UI")]
    public GameObject deathPanel;

    private bool uiInitialized;

    private void Awake()
    {
        if (photonView.IsMine)
            LocalPlayer = this;
    }

    private void Start()
    {
        if (photonView.IsMine)
            StartCoroutine(InitializeUI());
    }

    IEnumerator InitializeUI()
    {
        while (!uiInitialized)
        {
            ScoreRef scoreRef = FindAnyObjectByType<ScoreRef>(FindObjectsInactive.Include);
            if (scoreRef != null)
            {
                deathPanel = scoreRef.gameObject;
                deathPanel.SetActive(false);

                uiInitialized = true;

                Debug.Log("Death Panel Initialized.");
                yield break;
            }

            yield return null;
        }
    }

    //----------------------------------------------------
    // NORMAL ZOMBIE
    //----------------------------------------------------

    public void AddZombieKill()
    {
        AddKill(false);
    }

    //----------------------------------------------------

    public void AddBossKill()
    {
        AddKill(true);
    }

    //----------------------------------------------------

    void AddKill(bool boss)
    {
        if (!photonView.IsMine)
            return;

        zombieKills++;

        int reward = boss ? bossReward : zombieReward;

        coinsEarnedThisMatch += reward;

        KillBoard.myKills = zombieKills;

        AuthenticationManager.Instance.CurrentProfile.coins += reward;
        AuthenticationManager.Instance.CurrentProfile.totalMultiKills++;

        AuthenticationManager.Instance.SavePlayerProfile();

        photonView.RPC(nameof(RPC_UpdateStats),
            RpcTarget.AllBuffered,
            zombieKills,
            coinsEarnedThisMatch,
            AuthenticationManager.Instance.CurrentProfile.coins);
    }

    //----------------------------------------------------

    [PunRPC]
    void RPC_UpdateStats(int kills, int earnedCoins, int totalCoins)
    {
        zombieKills = kills;
        coinsEarnedThisMatch = earnedCoins;

        KillBoard.myKills = zombieKills;
        KillBoard.totalCoinsEarned = coinsEarnedThisMatch;

        Debug.Log(
            photonView.Owner.NickName +
            " | Kills : " + zombieKills +
            " | Match Coins : " + coinsEarnedThisMatch +
            " | Total Coins : " + totalCoins);

        if (KillBoard.Instance != null)
            KillBoard.Instance.Refresh();
    }

    //----------------------------------------------------

    public void ShowDeathUI()
    {
        if (!photonView.IsMine)
            return;

        if (deathPanel == null)
        {
            ScoreRef scoreRef = FindAnyObjectByType<ScoreRef>(FindObjectsInactive.Include);

            if (scoreRef != null)
            {
                deathPanel = scoreRef.gameObject;
            }
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
            Debug.Log("Death Panel Opened");
        }
        else
        {
            Debug.LogError("Death Panel not found.");
        }
    }

    //----------------------------------------------------

    public void ResetStats()
    {
        zombieKills = 0;
        coinsEarnedThisMatch = 0;
    }
}
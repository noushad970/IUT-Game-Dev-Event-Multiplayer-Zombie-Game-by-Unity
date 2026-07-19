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

    // Local player's match stats only
    public static int totalKillsMutli = 0;
    public static int bossKilledMulti = 0;
    public static int coinsEarnedThisMatchMulti = 0;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayer = this;

            totalKillsMutli = 0;
            bossKilledMulti = 0;
            coinsEarnedThisMatchMulti = 0;
        }
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
            ScoreRef scoreRef =
                FindAnyObjectByType<ScoreRef>(FindObjectsInactive.Include);

            if (scoreRef != null)
            {
                deathPanel = scoreRef.gameObject;
                deathPanel.SetActive(false);

                uiInitialized = true;
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
    // BOSS
    //----------------------------------------------------

    public void AddBossKill()
    {
        AddKill(true);
    }

    //----------------------------------------------------
    // ADD KILL
    //----------------------------------------------------

    void AddKill(bool boss)
    {
        // IMPORTANT:
        // Don't use photonView.IsMine here.
        // This is executed on the Master Client.

        zombieKills++;

        int reward = boss ? bossReward : zombieReward;

        coinsEarnedThisMatch += reward;

        photonView.RPC(
            nameof(RPC_UpdateStats),
            RpcTarget.AllBuffered,
            zombieKills,
            coinsEarnedThisMatch,
            boss);
    }

    //----------------------------------------------------

    [PunRPC]
    void RPC_UpdateStats(int kills, int earnedCoins, bool boss)
    {
        zombieKills = kills;
        coinsEarnedThisMatch = earnedCoins;

        // Update ONLY the local player's UI/statics
        if (photonView.IsMine)
        {
            totalKillsMutli = zombieKills;
            coinsEarnedThisMatchMulti = coinsEarnedThisMatch;

            if (boss)
                bossKilledMulti++;

            KillBoard.myKills = totalKillsMutli;
            KillBoard.totalCoinsEarned = coinsEarnedThisMatchMulti;
        }

        Debug.Log(
            photonView.Owner.NickName +
            " | Kills : " + zombieKills +
            " | Match Coins : " + coinsEarnedThisMatch);

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
            ScoreRef scoreRef =
                FindAnyObjectByType<ScoreRef>(FindObjectsInactive.Include);

            if (scoreRef != null)
                deathPanel = scoreRef.gameObject;
        }

        if (deathPanel != null)
            deathPanel.SetActive(true);
    }

    //----------------------------------------------------

    public void ResetStats()
    {
        zombieKills = 0;
        coinsEarnedThisMatch = 0;

        if (photonView.IsMine)
        {
            totalKillsMutli = 0;
            bossKilledMulti = 0;
            coinsEarnedThisMatchMulti = 0;

            KillBoard.myKills = 0;
            KillBoard.totalCoinsEarned = 0;
        }
    }
}
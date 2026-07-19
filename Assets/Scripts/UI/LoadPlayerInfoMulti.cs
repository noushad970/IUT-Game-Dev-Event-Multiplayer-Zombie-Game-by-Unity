using System.Collections;
using TMPro;
using UnityEngine;

public class LoadPlayerInfoMulti : MonoBehaviour
{
    [Header("Player Info")]
    public TextMeshProUGUI totalCoinEarned, totalKill;

    private void Start()
    {
        loadPlayerInfo();

    }
    private void OnEnable()
    {
        loadPlayerInfo();
    }

    public void loadPlayerInfo()
    {
        totalCoinEarned.text="Total coins earned: "+PlayerStatsMulti.coinsEarnedThisMatchMulti.ToString();
        totalKill.text = "Total kills: " + PlayerStatsMulti.totalKillsMutli.ToString();
        AuthenticationManager.Instance.CurrentProfile.coins += PlayerStatsMulti.coinsEarnedThisMatchMulti;
        AuthenticationManager.Instance.CurrentProfile.totalMultiKills += PlayerStatsMulti.totalKillsMutli;
        AuthenticationManager.Instance.SavePlayerProfile();
    }
    
}

using System.Collections;
using TMPro;
using UnityEngine;

public class LoadPlayerInfo : MonoBehaviour
{
    [Header("Player Info")]
    public TextMeshProUGUI playernameText, playerCoinText;
    private void Start()
    {
        StartCoroutine(loadwait());
    }
    public void loadPlayerInfo()
    {
        playernameText.text = AuthenticationManager.Instance.CurrentProfile.username;
        playerCoinText.text = AuthenticationManager.Instance.CurrentProfile.coins.ToString();
    }
    IEnumerator loadwait()
    {
        yield return new WaitForSeconds(4f);
        playernameText.text = AuthenticationManager.Instance.CurrentProfile.username;
        playerCoinText.text = AuthenticationManager.Instance.CurrentProfile.coins.ToString();
    }
}

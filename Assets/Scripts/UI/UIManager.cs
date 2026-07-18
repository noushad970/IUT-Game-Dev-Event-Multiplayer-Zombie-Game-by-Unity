using UnityEngine;
using System.Collections;
public class UIManager : MonoBehaviour
{
    public GameObject maleChar, femaleChar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(loadplayer());
        KillBoard.myKills = 0;
        KillBoard.totalCoinsEarned = 0;
    }

    IEnumerator loadplayer()
    {
        yield return new WaitForSeconds(4f);
        if(AuthenticationManager.Instance.CurrentProfile.selectedCharacter == 2)
        {
            maleChar.SetActive(true);
            femaleChar.SetActive(false);
        }
        else
        {
            maleChar.SetActive(false);
            femaleChar.SetActive(true);
        }
    }
    public void loadPlayerAgain()
    {
        if (AuthenticationManager.Instance.CurrentProfile.selectedCharacter == 2)
        {
            maleChar.SetActive(true);
            femaleChar.SetActive(false);
        }
        else
        {
            maleChar.SetActive(false);
            femaleChar.SetActive(true);
        }
    }
}

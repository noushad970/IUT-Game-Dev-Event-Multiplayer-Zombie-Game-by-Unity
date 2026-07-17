using System.Collections;
using UnityEngine;

public class AutoLogin : MonoBehaviour
{
    public GameObject authPanel;
    public GameObject mainMenuPanel;

    [Header("Delay")]
    public float splashDelay = 1.5f;

    IEnumerator Start()
    {
        while (AuthenticationManager.Instance == null ||
               !AuthenticationManager.Instance.IsInitialized)
        {
            yield return null;
        }

        yield return new WaitForSeconds(splashDelay);

        if (PlayerPrefs.HasKey("EMAIL") &&
            PlayerPrefs.HasKey("PASSWORD"))
        {
            string email = PlayerPrefs.GetString("EMAIL");
            string password = PlayerPrefs.GetString("PASSWORD");

            Debug.Log("Trying Auto Login...");

            var task = AuthenticationManager.Instance.Login(email, password);

            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result)
            {
                Debug.Log("Auto Login Success");

                mainMenuPanel.SetActive(true);
                authPanel.SetActive(false);
            }
            else
            {
                Debug.Log("Auto Login Failed");

                authPanel.SetActive(true);
                mainMenuPanel.SetActive(false);
            }
        }
        else
        {
            Debug.Log("No Previous Login Found");

            authPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
        }
    }
}
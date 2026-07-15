using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLogin : MonoBehaviour
{
    public GameObject authPanel, mainMenuPanel;

    [Header("Delay")]
    public float splashDelay = 1.5f;

    private IEnumerator Start()
    {
        // Wait until AuthenticationManager has initialized Firebase
        while (AuthenticationManager.Instance == null ||
               !AuthenticationManager.Instance.IsInitialized)
        {
            yield return null;
        }

        // Optional splash screen delay
        yield return new WaitForSeconds(splashDelay);

        if (AuthenticationManager.Instance.IsLoggedIn())
        {
            Debug.Log("Auto Login Success");
            Debug.Log("UID : " + AuthenticationManager.Instance.GetUID());

            // TODO:
            // Load player profile from Firestore here if needed.

            mainMenuPanel.SetActive(true);
            authPanel.SetActive(false);
        }
        else
        {
            Debug.Log("No Previous Login Found");

            mainMenuPanel.SetActive(false);
            authPanel.SetActive(true);
        }
    }
}
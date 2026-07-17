using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    [Header("PlayFab")]
    public string titleId = "YOUR_TITLE_ID";

    private void Start()
    {
        PlayFabSettings.staticSettings.TitleId = titleId;

        Debug.Log("Initializing PlayFab...");

        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = false
            },
            OnLoginSuccess,
            OnLoginFailure
        );
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("✅ PlayFab Initialized Successfully!");
        Debug.Log("PlayFab ID: " + result.PlayFabId);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("❌ PlayFab Initialization Failed");
        Debug.LogError(error.GenerateErrorReport());
    }

}
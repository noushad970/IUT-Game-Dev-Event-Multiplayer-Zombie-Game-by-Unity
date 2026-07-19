using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    [Header("Button")]
    public Button loginButton;

    // Load Main Menu
    [Header("Panels")]
    public GameObject authenticationPanel, MainMenuPanel;
    public LoadPlayerInfo loadPlayerInfo;
    public TextMeshProUGUI notificationText;
    private void Awake()
    {
        loginButton.onClick.AddListener(Login);
    }

    private void OnDestroy()
    {
        if (loginButton != null)
            loginButton.onClick.RemoveListener(Login);
    }

    public async void Login()
    {
        string username = usernameInput.text.Trim().ToLower();
        string password = passwordInput.text;

        Debug.Log("========== LOGIN ==========");

        // Username validation
        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.LogError("Username cannot be empty.");
            showNotification("Username cannot be empty.");
            return;
        }

        if (username.Length < 3)
        {
            Debug.LogError("Username must be at least 3 characters.");
            showNotification("Username must be at least 3 characters.");
            return;
        }

        // Password validation
        if (string.IsNullOrWhiteSpace(password))
        {
            Debug.LogError("Password cannot be empty.");
            showNotification("Password cannot be empty.");
            return;
        }

        // Generate Firebase email
        string email = username + "@gmail.com";

        Debug.Log("Username : " + username);
        Debug.Log("Generated Email : " + email);

        bool success = await AuthenticationManager.Instance.Login(
            email,
            password
        );

        if (success)
        {
            Debug.Log("Login Successful.");
            showNotification("Login Successful.");
            loadPlayerInfo.loadPlayerInfo();
            // TODO:
            // Load player profile from Firestore
            // SceneManager.LoadScene("MainMenu");
            MainMenuPanel.SetActive(true);
            authenticationPanel.SetActive(false);


        }
        else
        {
            Debug.LogError("Login Failed.");
            showNotification("Login failed, Please try again.....");
        }
    }
    void showNotification(string message)
    {
        notificationText.gameObject.SetActive(true);
        notificationText.text = message;
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterUI : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;

    [Header("Buttons")]
    public Button signupButton;
    public LoadPlayerInfo loadPlayerInfo;
    public TextMeshProUGUI notificationText;
    private void Awake()
    {
        signupButton.onClick.AddListener(Register);
    }

    private void OnDestroy()
    {
        signupButton.onClick.RemoveListener(Register);
    }

    public async void Register()
    {
        string username = usernameInput.text.Trim().ToLower();
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        Debug.Log("========== SIGNUP ==========");

        // Username check
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

        if (username.Length > 20)
        {
            Debug.LogError("Username cannot exceed 20 characters.");
            showNotification("Username cannot exceed 20 characters.");
            return;
        }

        // Only letters, numbers and underscore
        foreach (char c in username)
        {
            if (!(char.IsLetterOrDigit(c) || c == '_'))
            {
                Debug.LogError("Username can only contain letters, numbers and underscore.");
                showNotification("Username can only contain letters, numbers and underscore.");
                return;
            }
        }

        // Password checks
        if (string.IsNullOrWhiteSpace(password))
        {
            Debug.LogError("Password cannot be empty.");
            showNotification("Password cannot be empty.");
            return;
        }

        if (password.Length < 6)
        {
            Debug.LogError("Password must contain at least 6 characters.");
            showNotification("Password must contain at least 6 characters.");
            return;
        }

        if (password != confirmPassword)
        {
            Debug.LogError("Passwords do not match.");
            showNotification("Passwords do not match.");
            return;
        }

        // Generate Firebase Email
        string email = username + "@gmail.com";

        Debug.Log("Username : " + username);
        Debug.Log("Generated Email : " + email);

        bool success = await AuthenticationManager.Instance.Register(
            email,
            password,
            username
        );

        if (success)
        {
            Debug.Log("Signup Successful.");
            showNotification("Signup Successful. Please wait while we load your profile...");
            // Open Character Selection
            AuthenticationUIManager.Instance.ShowCharacterSelection();
            loadPlayerInfo.loadPlayerInfo();
        }
        else
        {
            Debug.LogError("Signup Failed.");
            showNotification("Signup failed, Please try again.....");
        }
    }
    void showNotification(string message)
    {
        notificationText.gameObject.SetActive(true);
        notificationText.text = message;
    }
}
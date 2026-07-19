using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AuthenticationUIManager : MonoBehaviour
{
    public static AuthenticationUIManager Instance;
    public LoadPlayerInfo loadPlayerInfo;

    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject signupPanel;
    public GameObject characterSelectionPanel;
    public GameObject menuPanel;

    [Header("Navigation Buttons")]
    public Button gotoSignupButton;
    public Button gotoLoginButton;

    [Header("Selected Character")]
    public int selectedCharacter = 1;
    public Button signoutButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Register navigation button events
        if (gotoSignupButton != null)
            gotoSignupButton.onClick.AddListener(GotoSignup);

        if (gotoLoginButton != null)
            gotoLoginButton.onClick.AddListener(GotoLogin);

        if (signoutButton != null)
            signoutButton.onClick.AddListener(SignOut);

        ShowLoginPanel();
    }
    void SignOut()
    {
        AuthenticationManager.Instance.Logout();
    }

    #region Panel Control

    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        characterSelectionPanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    public void ShowSignupPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        characterSelectionPanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    public void ShowCharacterSelection()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        characterSelectionPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void ShowMenuPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        characterSelectionPanel.SetActive(false);
        menuPanel.SetActive(false);
        loadPlayerInfo.loadPlayerInfo();
    }

    #endregion

    #region Navigation

    // Signup button in Login Panel
    public void GotoSignup()
    {
        ShowSignupPanel();
    }

    // Login button in Signup Panel
    public void GotoLogin()
    {
        ShowLoginPanel();
    }

    #endregion

    #region Character Selection

    public void SelectCharacter1()
    {
        selectedCharacter = 1;
        Debug.Log("Character 1 Selected");
    }

    public void SelectCharacter2()
    {
        selectedCharacter = 2;
        Debug.Log("Character 2 Selected");
    }

    #endregion

    #region Continue

    public void ContinueCharacterSelection()
    {
        Debug.Log("Selected Character : " + selectedCharacter);

        // TODO:
        // FirebaseManager.Instance.CreatePlayerProfile(...);

        // SceneManager.LoadScene("MainMenu");
    }

    #endregion
}
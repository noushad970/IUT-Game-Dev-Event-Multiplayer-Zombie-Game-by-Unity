using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{

    [Header("Buttons")]
    [SerializeField] private Button character1Button;
    [SerializeField] private Button character2Button;
    [SerializeField] private Button continueButton;

    [Header("Selection Indicator")]
    [SerializeField] private GameObject character1Selected;
    [SerializeField] private GameObject character2Selected;
    public UIManager manager;
    public int SelectedCharacterID { get; private set; } = 1;

    public event Action<int> OnCharacterConfirmed;

    [Header("Panels")]
    public GameObject authenticationPanel;
    public GameObject MainMenuPanel,menuButtons;

    private void Awake()
    {
       

        character1Button.onClick.AddListener(() => SelectCharacter(1));
        character2Button.onClick.AddListener(() => SelectCharacter(2));
        continueButton.onClick.AddListener(ConfirmSelection);
    }

    private void OnEnable()
    {
        SelectCharacter(1);
    }

    public void SelectCharacter(int characterID)
    {
        SelectedCharacterID = characterID;

        if (character1Selected != null)
            character1Selected.SetActive(characterID == 1);

        if (character2Selected != null)
            character2Selected.SetActive(characterID == 2);

        Debug.Log("Character " + characterID + " Selected");
    }

    private void ConfirmSelection()
    {
        continueButton.interactable = false;

        Debug.Log("Saving selected character...");

        AuthenticationManager auth = AuthenticationManager.Instance;

        if (auth == null)
        {
            Debug.LogError("AuthenticationManager not found.");
            continueButton.interactable = true;
            return;
        }

        if (!auth.IsLoggedIn())
        {
            Debug.LogError("No logged in user.");
            continueButton.interactable = true;
            return;
        }

        if (auth.CurrentProfile == null)
        {
            Debug.LogError("Player profile not loaded.");
            continueButton.interactable = true;
            return;
        }

        // Update local profile
        auth.CurrentProfile.selectedCharacter = SelectedCharacterID;

        // Save to PlayFab
        auth.SavePlayerProfile();

        Debug.Log("Character saved successfully.");

        OnCharacterConfirmed?.Invoke(SelectedCharacterID);

        // Show Main Menu
        MainMenuPanel.SetActive(true);
        menuButtons.SetActive(true);
        authenticationPanel.SetActive(false);
        manager.loadPlayerAgain();
        continueButton.interactable = true;
    }

    public int GetSelectedCharacter()
    {
        return SelectedCharacterID;
    }
}
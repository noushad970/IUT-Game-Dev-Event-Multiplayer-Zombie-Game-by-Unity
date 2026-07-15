using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Firestore;

public class CharacterSelectionUI : MonoBehaviour
{
    public static CharacterSelectionUI Instance;

    [Header("Buttons")]
    [SerializeField] private Button character1Button;
    [SerializeField] private Button character2Button;
    [SerializeField] private Button continueButton;

    [Header("Selection Indicator")]
    [SerializeField] private GameObject character1Selected;
    [SerializeField] private GameObject character2Selected;

    public int SelectedCharacterID { get; private set; } = 1;

    public event Action<int> OnCharacterConfirmed;
    [Header("Panels")]
    public GameObject authenticationPanel, MainMenuPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

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

        Debug.Log($"Character {characterID} Selected");
    }

    private async void ConfirmSelection()
    {
        continueButton.interactable = false;

        Debug.Log("Saving selected character...");

        try
        {
            string uid = AuthenticationManager.Instance.GetUID();

            if (string.IsNullOrEmpty(uid))
            {
                Debug.LogError("No logged in user found.");

                continueButton.interactable = true;
                return;
            }

            DocumentReference doc =
                AuthenticationManager.Instance
                .Firestore
                .Collection("players")
                .Document(uid);

            Dictionary<string, object> updates =
                new Dictionary<string, object>()
                {
                    { "selectedCharacter", SelectedCharacterID }
                };

            await doc.UpdateAsync(updates);

            Debug.Log("Character saved successfully.");

            OnCharacterConfirmed?.Invoke(SelectedCharacterID);

            // Load Main Menu
            MainMenuPanel.SetActive(true);
            authenticationPanel.SetActive(false);

        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save character.");
            Debug.LogException(e);
        }

        continueButton.interactable = true;
    }

    public int GetSelectedCharacter()
    {
        return SelectedCharacterID;
    }
}
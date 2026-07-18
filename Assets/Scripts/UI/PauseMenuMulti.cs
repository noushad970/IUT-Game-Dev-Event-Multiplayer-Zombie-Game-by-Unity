using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuMulti : MonoBehaviourPunCallbacks
{
    public static PauseMenuMulti Instance;

    [Header("Panels")]
    public GameObject pausePanel;

    [Header("Player Components")]
    public MonoBehaviour movement;
    public MonoBehaviour weapon;
    public MonoBehaviour grenade;

    private bool isPaused;

    private void Awake()
    {
        Instance = this;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Start()
    {
        FindLocalPlayer();
    }

    void FindLocalPlayer()
    {
        PhotonView[] players = FindObjectsByType<PhotonView>(FindObjectsSortMode.None);

        foreach (PhotonView view in players)
        {
            if (!view.IsMine)
                continue;

            PlayerMovementMulti move = view.GetComponent<PlayerMovementMulti>();
            if (move != null)
                movement = move;

            PlayerWeaponMulti weaponScript = view.GetComponent<PlayerWeaponMulti>();
            if (weaponScript != null)
                weapon = weaponScript;

            PlayerGrenadeMulti grenadeScript = view.GetComponent<PlayerGrenadeMulti>();
            if (grenadeScript != null)
                grenade = grenadeScript;

            break;
        }
    }

    //========================================
    // Pause
    //========================================
    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (movement != null)
            movement.enabled = false;

        if (weapon != null)
            weapon.enabled = false;

        if (grenade != null)
            grenade.enabled = false;

        

        Debug.Log("Game Paused");
    }

    //========================================
    // Resume
    //========================================
    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (movement != null)
            movement.enabled = true;

        if (weapon != null)
            weapon.enabled = true;

        if (grenade != null)
            grenade.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Game Resumed");
    }

    //========================================
    // Leave Match
    //========================================
    public void GoToMainMenu()
    {
        Debug.Log("Leaving Room...");

        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
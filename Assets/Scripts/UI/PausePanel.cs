using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    public static PausePanel Instance;

    [Header("UI")]
    public GameObject pausePanel;

    [Header("Buttons")]
    public Button pauseButton;
    public Button resumeButton;
    public Button mainMenuButton;

    private bool isPaused;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        pausePanel.SetActive(false);

        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(MainMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    //--------------------------------------------------

    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;

        pausePanel.SetActive(true);

        Time.timeScale = 0f;

        AudioListener.pause = true;
    }

    //--------------------------------------------------

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;

        pausePanel.SetActive(false);

        Time.timeScale = 1f;

        AudioListener.pause = false;
    }

    //--------------------------------------------------

    public void MainMenu()
    {
        Time.timeScale = 1f;

        AudioListener.pause = false;

        SceneManager.LoadScene("Main Menu");
    }
}
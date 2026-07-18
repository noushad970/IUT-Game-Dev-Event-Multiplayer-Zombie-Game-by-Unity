using TMPro;
using UnityEngine;

public class GameTimerOffline : MonoBehaviour
{
    public static GameTimerOffline Instance;

    [Header("Timer")]
    public float gameTime = 300f;

    [Header("UI")]
    public TMP_Text timerText;

    public GameObject scorePanel;

    private bool gameFinished;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameFinished = false;

        if (scorePanel != null)
            scorePanel.SetActive(false);
    }

    private void Update()
    {
        if (gameFinished)
            return;

        gameTime -= Time.deltaTime;

        if (gameTime < 0)
            gameTime = 0;

        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";

        if (gameTime <= 0)
        {
            EndGame();
        }
    }

    //----------------------------------------------------

    void EndGame()
    {
        gameFinished = true;

        Debug.Log("Game Finished");

        Time.timeScale = 0f;

        if (scorePanel != null)
            scorePanel.SetActive(true);
    }

    //----------------------------------------------------

    public void RestartTimer()
    {
        gameFinished = false;

        gameTime = 300f;

        Time.timeScale = 1f;

        if (scorePanel != null)
            scorePanel.SetActive(false);
    }
}
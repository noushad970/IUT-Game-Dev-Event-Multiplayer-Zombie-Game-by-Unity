using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameTimerOffline : MonoBehaviour
{
    public static GameTimerOffline Instance;

    [Header("Timer")]
    public float gameTime = 300f;

    [Header("UI")]
    public TMP_Text timerText,totalKills,finalKills,finalScore;

    public GameObject scorePanel;
    public Button mainMenuButton;

    private bool gameFinished;
    public Slider healthBar;

    public int maxHealth = 100;
    private int currentHealth;

    

    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameFinished = false;
        currentHealth = maxHealth;

        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        if (scorePanel != null)
            scorePanel.SetActive(false);
        mainMenuButton.onClick.AddListener(gotoMainMenu);
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
        healthBar.value = PlayerHealth.healthValue ;
        timerText.text = $"{minutes:00}:{seconds:00}";
        totalKills.text = "Kills : " + ZombieSpawner.offlineZombieDiedCount;
        if (gameTime <= 0)
        {
            EndGame();
        }
        if(EnemyHealth.isBossDead)
        {
            EndGame();
            EnemyHealth.isBossDead = false;
        }if(PlayerHealth.isPlayerDead)
        {
            EndGame();
            PlayerHealth.isPlayerDead = false;
        }
    } 

    //----------------------------------------------------

    void EndGame()
    {
        gameFinished = true;

        Debug.Log("Game Finished");

        //Time.timeScale = 0f;

        if (scorePanel != null)
            scorePanel.SetActive(true);
        totalKills.text = "Kills : " + ZombieSpawner.offlineZombieDiedCount;
        finalScore.text = "Final Score : " + ZombieSpawner.totalCoinEarned;
        finalKills.text = "Total Kills : " + ZombieSpawner.offlineZombieDiedCount;
        AuthenticationManager.Instance.CurrentProfile.coins += ZombieSpawner.totalCoinEarned;
        AuthenticationManager.Instance.CurrentProfile.totalMultiKills += ZombieSpawner.offlineZombieDiedCount;
        AuthenticationManager.Instance.SavePlayerProfile();
        //AuthenticationManager.Instance.GetSelectedPlayer().ContinueWith(task =>
        //{
        //    if (task.IsCompletedSuccessfully)
        //    {
        //        int selectedPlayer = task.Result;
        //        PlayerProfile profile = AuthenticationManager.Instance.CurrentProfile;
        //        if (profile != null)
        //        {
        //            profile.totalMultiKills += ZombieSpawner.offlineZombieDiedCount;
        //            profile.coins += ZombieSpawner.totalCoinEarned;
        //            AuthenticationManager.Instance.SavePlayerProfile();
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Failed to get selected player: " + task.Exception);
        //    }
        //});
        ZombieSpawner.offlineZombieDiedCount = 0;
        ZombieSpawner.totalCoinEarned = 0;
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
    void gotoMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        gameFinished = false;

        gameTime = 300f;

        Time.timeScale = 1f;

        if (scorePanel != null)
            scorePanel.SetActive(false);
    }
}
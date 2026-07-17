using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button startSinglePlayButton;
    [Header("Lobby UI")]
    public GameObject lobbyPanel, mainmenuPanel;
    public Button multiplayerButton, backButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject lobbyhome, lobbylist, RoomPanel;
    void Start()
    {
        startSinglePlayButton.onClick.AddListener(startSinglePlay);  
        multiplayerButton.onClick.AddListener(onclickMultiplayerButton);
        backButton.onClick.AddListener(gotoHome);
        gotoHome();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void startSinglePlay()
    {
        SceneManager.LoadScene("SinglePlay");
    }
    void onclickMultiplayerButton()
    {
        lobbyPanel.SetActive(true);
        mainmenuPanel.SetActive(false);
        lobbyhome.SetActive(true);
        lobbylist.SetActive(false);
        RoomPanel.SetActive(false);
    }
    void onClickBackButton()
    {
        mainmenuPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }
    void gotoHome()
    {
        mainmenuPanel.SetActive(true);
        lobbyPanel.SetActive(false);

    }
}

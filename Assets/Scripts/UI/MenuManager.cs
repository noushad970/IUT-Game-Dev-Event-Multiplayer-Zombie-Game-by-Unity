using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button startSinglePlayButton;
    [Header("Lobby UI")]
    public GameObject lobbyPanel, mainmenuPanel;
    public Button multiplayerButton, backButton, characterSelectionButton, backfromCharSelectionbutton,exitButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject lobbyhome, lobbylist, RoomPanel,characterSelectionPanel;
    void Start()
    {
        startSinglePlayButton.onClick.AddListener(startSinglePlay);  
        multiplayerButton.onClick.AddListener(onclickMultiplayerButton);
        backButton.onClick.AddListener(gotoHome);
        characterSelectionButton.onClick.AddListener(gotoCharSelection);
        gotoHome();
        backfromCharSelectionbutton.onClick.AddListener(gotoHome);
        exitButton.onClick.AddListener(clickExitButton);
    }
    void clickExitButton()
    {
        Application.Quit();
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
        characterSelectionPanel.SetActive(false);
    }
    void gotoHome()
    {
        mainmenuPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        characterSelectionPanel.SetActive(false);
    }
    void gotoCharSelection()
    {
        characterSelectionPanel.SetActive(true);
        mainmenuPanel.SetActive(false);
        lobbyhome.SetActive(false);
    }
}

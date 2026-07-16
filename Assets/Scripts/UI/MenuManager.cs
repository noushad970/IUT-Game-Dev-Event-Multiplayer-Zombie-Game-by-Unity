using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button startSinglePlayButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startSinglePlayButton.onClick.AddListener(startSinglePlay);  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void startSinglePlay()
    {
        SceneManager.LoadScene("SinglePlay");
    }
}

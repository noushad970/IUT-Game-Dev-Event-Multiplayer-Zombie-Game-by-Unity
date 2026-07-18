using UnityEngine;
using UnityEngine.UI;

public class AboutPanelUI : MonoBehaviour
{
    public Button aboutButton, backButton;
    public GameObject aboutPanel;
    private void Start()
    {
        aboutPanel.SetActive(false);
        aboutButton.onClick.AddListener(openAboutPanel);
        backButton.onClick.AddListener(closeAboutPanel);
    }
    void openAboutPanel()
    {
        aboutPanel.SetActive(true);
    }
    void closeAboutPanel()
    {
        aboutPanel.SetActive(false);
    }
}

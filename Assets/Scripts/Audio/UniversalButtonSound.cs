using UnityEngine;
using UnityEngine.UI;

public class UniversalButtonSound : MonoBehaviour
{
    public static UniversalButtonSound Instance;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClick;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        RegisterAllButtons();
    }

    public void RegisterAllButtons()
    {
        Button[] buttons =
            FindObjectsByType<Button>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(PlayClick);
            btn.onClick.AddListener(PlayClick);
        }

        Debug.Log("Registered " + buttons.Length + " buttons.");
    }

    public void PlayClick()
    {
        if (audioSource == null)
            return;

        if (buttonClick == null)
            return;

        audioSource.PlayOneShot(buttonClick);
    }
}
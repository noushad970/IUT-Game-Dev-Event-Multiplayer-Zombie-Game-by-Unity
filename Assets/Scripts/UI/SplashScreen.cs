using System.Collections;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    public GameObject splashScreen;
    [Header("Rotation")]
    public float rotationSpeed = 180f; // Degrees per second

    public RectTransform rectTransform;
    private void Start()
    {
        splashScreen.SetActive(true);
        StartCoroutine(loading());
    }
    IEnumerator loading()
    {
        yield return new WaitForSeconds(4f);
        splashScreen.SetActive(false);
    }
    void Update()
    {
        rectTransform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}

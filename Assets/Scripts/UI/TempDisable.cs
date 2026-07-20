using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TempDisable : MonoBehaviour
{
    public GameObject target;
    public Button closeButton;
    private void Awake()
    {
       DisableForOneSecond();
        closeButton.onClick.AddListener(onclickCloseButton);
    }
    public void DisableForOneSecond()
    {
        StartCoroutine(DisableRoutine());
    }

    IEnumerator DisableRoutine()
    {
        target.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        target.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        target.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        target.SetActive(false);
        onclickCloseButton();

    }
    void onclickCloseButton()
    {
        target.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }
} 
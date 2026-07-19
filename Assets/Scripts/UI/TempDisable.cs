using System.Collections;
using UnityEngine;

public class TempDisable : MonoBehaviour
{
    public GameObject target;
    private void Start()
    {
       DisableForOneSecond();
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

    }
} 
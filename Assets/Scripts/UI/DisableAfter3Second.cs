using System.Collections;
using UnityEngine;

public class DisableAfter3Second : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        StartCoroutine(wait());
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(3f);
        this.gameObject.SetActive(false);
    }
}

using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    Coroutine shakeRoutine;
    public static bool shakeCamera = false;
    void Awake()
    {
        Instance = this;
    }

    //----------------------------------------------------
    // Call this
    //----------------------------------------------------
    private void Update()
    {
        if(shakeCamera)
        {
            Shake();
            shakeCamera = false;
        }
    }
    public void Shake(float duration = 0.25f, float magnitude = 0.12f)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Save the camera position AFTER follow script moved it
            Vector3 basePos = transform.localPosition;

            // Small random offset
            Vector3 offset = Random.insideUnitSphere * magnitude;
            offset.z = 0;

            transform.localPosition = basePos + offset;

            yield return null;

            // Restore immediately before next frame
            transform.localPosition = basePos;
        }

        shakeRoutine = null;
    }
}
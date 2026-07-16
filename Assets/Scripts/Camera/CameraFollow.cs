using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Offset")]
    public Vector3 offset = new Vector3(0f, 10f, -7f);

    [Header("Follow Settings")]
    public float followSpeed = 10f;

   // private readonly Quaternion fixedRotation = Quaternion.Euler(70.4f, 90f, 0f);

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Smooth follow
        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        // Always keep the same rotation
     //   transform.rotation = fixedRotation;
    }
}
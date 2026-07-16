using UnityEngine;

public class PlayerFollowCamera : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;

    private void FixedUpdate()
    {
        if (cameraTransform == null)
            return;

        Vector3 rotation = transform.eulerAngles;
        rotation.y = cameraTransform.eulerAngles.y;

        transform.eulerAngles = rotation;
    }
}
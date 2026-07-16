using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerGrenade : MonoBehaviour
{
    [Header("Grenade")]
    public GameObject grenadePrefab;      // Disabled prefab/child
    public Transform throwPoint;
    public Transform throwDirection;

    [Header("Throw Settings")]
    public float throwForce = 15f;
    public float upwardForce = 3f;

    [Header("Cooldown")]
    public float cooldown = 20f;

    private float nextThrowTime;

    private Button grenadeButton;

    private void Start()
    {
        FindGrenadeButton();
    }

    private void Update()
    {
        // PC Right Mouse Button
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            ThrowGrenade();
        }
    }

    void FindGrenadeButton()
    {
        GrenadeRef grenadeRef = FindFirstObjectByType<GrenadeRef>();

        if (grenadeRef == null)
        {
            Debug.Log("Grenade Button not found. (PC mode only)");
            return;
        }

        grenadeButton = grenadeRef.GetComponent<Button>();

        if (grenadeButton != null)
        {
            grenadeButton.onClick.AddListener(ThrowGrenade);
            Debug.Log("Grenade Button Initialized.");
        }
    }

    public void ThrowGrenade()
    {
        // Cooldown Check
        if (Time.time < nextThrowTime)
        {
            Debug.Log(
                $"Grenade Cooldown: {(nextThrowTime - Time.time):F1} sec"
            );
            return;
        }

        if (grenadePrefab == null)
        {
            Debug.LogError("Grenade Prefab is missing!");
            return;
        }

        if (throwPoint == null || throwDirection == null)
        {
            Debug.LogError("Throw Point or Throw Direction is not assigned!");
            return;
        }

        nextThrowTime = Time.time + cooldown;

        // Spawn grenade
        GameObject grenade = Instantiate(
            grenadePrefab,
            throwPoint.position,
            Quaternion.identity
        );

        grenade.SetActive(true);

        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 throwVector =
                throwDirection.forward * throwForce +
                Vector3.up * upwardForce;

            rb.AddForce(throwVector, ForceMode.Impulse);
        }

        Debug.Log("Grenade Thrown");
    }

    public float GetRemainingCooldown()
    {
        return Mathf.Max(0f, nextThrowTime - Time.time);
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerGrenadeMulti : MonoBehaviourPun
{
    [Header("Grenade")]
    public GameObject grenadePrefab;      // Must be in Resources folder
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
        if (photonView.IsMine)
        {
            FindGrenadeButton();
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

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
        if (!photonView.IsMine) return;

        // Cooldown Check
        if (Time.time < nextThrowTime)
        {
            Debug.Log($"Grenade Cooldown: {(nextThrowTime - Time.time):F1} sec");
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

        // Spawn grenade using Photon (works in both offline & multiplayer)
        Vector3 spawnPos = throwPoint.position;
        Quaternion spawnRot = Quaternion.identity;

        GameObject grenade = PhotonNetwork.Instantiate(
            grenadePrefab.name,
            spawnPos,
            spawnRot
        );

        // Apply force only on the owner
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 throwVector = throwDirection.forward * throwForce + Vector3.up * upwardForce;
            rb.AddForce(throwVector, ForceMode.Impulse);
        }

        Debug.Log("Grenade Thrown");
    }

    public float GetRemainingCooldown()
    {
        return Mathf.Max(0f, nextThrowTime - Time.time);
    }
}
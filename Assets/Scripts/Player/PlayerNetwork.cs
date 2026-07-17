using Photon.Pun;
using UnityEngine;

public class PlayerNetwork : MonoBehaviourPun
{
    public Camera playerCamera;
    public AudioListener listener;

    public PlayerMovementMulti movement;
    public PlayerWeaponMulti weapon;
    public PlayerGrenadeMulti grenade;


    private void Start()
    {
        FindMobileUI();

        if (!photonView.IsMine)
        {
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);

            if (listener != null)
                listener.enabled = false;

            if (movement != null)
                movement.enabled = false;

            if (weapon != null)
                weapon.enabled = false;

            if (grenade != null)
                grenade.enabled = false;

        }
        
           
        
    }

    private void FindMobileUI()
    {
        MobileUIRef uiRef = FindFirstObjectByType<MobileUIRef>();

        if (uiRef == null)
        {
            Debug.LogWarning("MobileUIRef not found.");
            return;
        }

       
        Debug.Log("Mobile UI Initialized.");
    }
}
using Photon.Pun;
using UnityEngine;

public class PlayerHealthMulti : MonoBehaviourPun
{
    [Header("Health")]
    public int maxHealth = 100;

    [SerializeField]
    private int currentHealth;

    public bool IsDead => isDead;

    [Header("Player Root")]
    public GameObject player;

    [Header("Death")]
    public float destroyDelay = 3f;

    private bool isDead;

    private Animator anim;
    private Collider playerCollider;

    private PlayerMovementMulti movement;
    private PlayerWeaponMulti weapon;
    private PlayerGrenadeMulti grenade;

    private void Awake()
    {
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();
        playerCollider = GetComponent<Collider>();

        movement = GetComponent<PlayerMovementMulti>();
        weapon = GetComponent<PlayerWeaponMulti>();
        grenade = GetComponent<PlayerGrenadeMulti>();
    }

    //=================================================
    // Called by Zombie
    //=================================================
    public void TakeDamage(int damage)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (isDead)
            return;

        currentHealth -= damage;

        Debug.Log(photonView.Owner.NickName +
                  " HP : " +
                  currentHealth);

        photonView.RPC(nameof(RPC_UpdateHealth), RpcTarget.All, currentHealth);

        if (currentHealth <= 0)
        {
            photonView.RPC(nameof(RPC_Die), RpcTarget.All);
        }
    }

    //=================================================

    [PunRPC]
    void RPC_UpdateHealth(int hp)
    {
        currentHealth = hp;
    }

    //=================================================

    [PunRPC]
    void RPC_Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(photonView.Owner.NickName + " Died");

        if (movement != null)
            movement.enabled = false;

        if (weapon != null)
            weapon.enabled = false;

        if (grenade != null)
            grenade.enabled = false;


        

        if (anim != null)
            anim.SetTrigger("Died");

        // Show local player's score panel only
        if (photonView.IsMine)
        {
            PlayerStatsMulti stats = GetComponent<PlayerStatsMulti>();

            if (stats != null)
                stats.ShowDeathUI();
        }

        Invoke(nameof(DestroyPlayer), destroyDelay);
    }

    //=================================================

    void DestroyPlayer()
    {
        if (!photonView.IsMine)
            return;

        PhotonNetwork.Destroy(player);
    }

    //=================================================

    public int GetHealth()
    {
        return currentHealth;
    }
}
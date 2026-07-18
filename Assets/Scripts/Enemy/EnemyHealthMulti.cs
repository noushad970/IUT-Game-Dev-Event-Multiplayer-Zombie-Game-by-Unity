using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealthMulti : MonoBehaviourPun
{
    public event Action<GameObject> OnZombieDeath;

    [Header("Spawner")]
    public ZombieSpawnerMulti spawner;

    [Header("Health")]
    public int maxHealth = 100;

    [SerializeField]
    private int currentHealth;

    [Header("Death")]
    public bool destroyOnDeath = true;
    public float destroyDelay = 3f;

    [Header("Events")]
    public UnityEvent onDeath;

    [Header("Effects")]
    public ParticleSystem bloodParticle;

    private bool isDead;

    private Animator anim;
    private Collider col;
    private EnemyAIMulti enemyAI;
    public bool isBoss = false;
    void Awake()
    {
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();
        col = GetComponent<Collider>();
        enemyAI = GetComponent<EnemyAIMulti>();

        FindSpawner();
    }

    void FindSpawner()
    {
        spawner = FindFirstObjectByType<ZombieSpawnerMulti>();
    }

    //=========================================================
    // DAMAGE
    //=========================================================
    public void TakeDamage(int damage, GameObject attacker)
    {
        // Only Master Client processes damage
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (isDead)
            return;

        currentHealth -= damage;

        photonView.RPC(nameof(RPC_PlayBlood), RpcTarget.All);

        Debug.Log($"{name} HP : {currentHealth}");

        if (currentHealth <= 0)
        {
            Die(attacker);
        }
    }

    //=========================================================
    // BLOOD EFFECT
    //=========================================================
    [PunRPC]
    void RPC_PlayBlood()
    {
        if (bloodParticle != null)
            bloodParticle.Play();
    }
    [PunRPC]
    public void RPC_RequestDamage(int damage, int attackerActor)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        Player attackerPlayer =
            PhotonNetwork.CurrentRoom.GetPlayer(attackerActor);

        GameObject attackerObject = null;

        PlayerStatsMulti[] players =
            FindObjectsByType<PlayerStatsMulti>(
                FindObjectsSortMode.None);

        foreach (PlayerStatsMulti p in players)
        {
            if (p.photonView.Owner == attackerPlayer)
            {
                attackerObject = p.gameObject;
                break;
            }
        }

        TakeDamage(damage, attackerObject);
    }
    //=========================================================
    // DIE
    //=========================================================
    void Die(GameObject attacker)
    {
        if (isDead)
            return;

        isDead = true;

        // Award kill to attacker
        if (attacker != null)
        {
            PlayerStatsMulti stats = attacker.GetComponentInChildren<PlayerStatsMulti>();

            if (stats != null)
            {
                if (isBoss)
                {
                    stats.AddBossKill();
                }
                else
                {
                    stats.AddZombieKill();
                }
            }
        }

        photonView.RPC(nameof(RPC_Die), RpcTarget.All);
    } 

    //=========================================================
    // DIE RPC
    //=========================================================
    [PunRPC]
    void RPC_Die()
    {
        isDead = true;

        Debug.Log(name + " Died");

        if (enemyAI != null)
            enemyAI.stopMovement();

        if (anim != null)
            anim.SetTrigger("Die");

        if (col != null)
            col.enabled = false;

        MonoBehaviour[] scripts =
            GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }

        OnZombieDeath?.Invoke(gameObject);

        onDeath?.Invoke();

        if (destroyOnDeath)
            Invoke(nameof(DestroyZombie), destroyDelay);
    }

    //=========================================================
    // DESTROY
    //=========================================================
    void DestroyZombie()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (spawner != null)
            spawner.ZombieKilled(gameObject);

        PhotonNetwork.Destroy(gameObject);
    }

    //=========================================================
    // GETTERS
    //=========================================================
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
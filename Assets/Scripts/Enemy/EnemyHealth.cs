using System;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    public event Action<GameObject> OnEnemyDeath;
    public ZombieSpawner spawner;
    [Header("Health")]
    public int maxHealth = 100;

    [SerializeField]
    private int currentHealth;

    [Header("Death")]
    public bool destroyOnDeath = true;
    public float destroyDelay = 3f;

    [Header("Events")]
    public UnityEvent onDeath;

    private bool isDead = false;

    public ParticleSystem bloodParticle;

    private void Awake()
    {
        currentHealth = maxHealth;
        FindSpawner();

    }
    void FindSpawner()
    {
        ZombieSpawner spawnerRef = FindFirstObjectByType<ZombieSpawner>();

        if (spawnerRef == null)
        {
            Debug.Log("Zombie Spawner not found.");
            return;
        } 

        spawner = spawnerRef;
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        Debug.Log($"{name} took {damage} damage. HP: {currentHealth}");
        bloodParticle.Play();

        if (currentHealth <= 0)
        {
            Die(attacker);
        }
    }
    private void Die(GameObject attacker)
    {
        if (isDead)
            return;

        isDead = true;
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<EnemyAI>().stopMovement();
        Debug.Log($"{name} died.");
        spawner.zombieDiedCount();
        // Give kill to attacker
        //if (attacker != null)
        //{
        //    PlayerStats stats = attacker.GetComponent<PlayerStats>();

        //    if (stats != null)
        //    {
        //        stats.AddKill();
        //    }
        //}

        // Play death animation
        Animator anim = GetComponent<Animator>();

        if (anim != null)
        {
            anim.Play("Die");
        }
        // Disable enemy behaviour
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }

        // Disable collider
        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = false;
        OnEnemyDeath?.Invoke(gameObject);
        onDeath?.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
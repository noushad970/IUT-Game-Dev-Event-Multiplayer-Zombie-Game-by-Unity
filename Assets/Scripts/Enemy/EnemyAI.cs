using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Detection")]
    public LayerMask playerLayer;
    public float detectionRadius = 50f;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;

    private NavMeshAgent agent;
    private Transform target;
    private PlayerHealth targetHealth;

    private Animator anim;

    private float attackTimer;
    public ParticleSystem hitEffect;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        Debug.Log($"[{name}] Enemy Initialized");
    }

    private void Update()
    {
        if (target == null)
        {
            SearchPlayer();
        }

        if (target == null)
        {
            Idle();
            return;
        }

        if (targetHealth == null || targetHealth.IsDead)
        {
            Debug.Log("Target lost or dead. Searching again...");

            target = null;
            targetHealth = null;
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);


        if (distance > attackRange)
        {
            Chase();
        }
        else
        {
            Attack();
        }
    }

    void SearchPlayer()
    {
        Collider[] players = Physics.OverlapSphere(
            transform.position,
            detectionRadius,
            playerLayer
        );

        Debug.Log("Players Found : " + players.Length);

        if (players.Length == 0)
            return;

        float nearestDistance = Mathf.Infinity;

        Transform nearestTarget = null;
        PlayerHealth nearestHealth = null;

        foreach (Collider c in players)
        {
            Debug.Log("Detected Collider : " + c.name);

            // Find PlayerHealth on self, parent or children
            PlayerHealth hp = c.GetComponent<PlayerHealth>();

            if (hp == null)
                hp = c.GetComponentInParent<PlayerHealth>();

            if (hp == null)
                hp = c.GetComponentInChildren<PlayerHealth>();

            if (hp == null)
            {
                Debug.Log("No PlayerHealth found for : " + c.name);
                continue;
            }

            if (hp.IsDead)
            {
                Debug.Log(hp.name + " is dead.");
                continue;
            }

            float distance = Vector3.Distance(
                transform.position,
                hp.transform.position
            );

            Debug.Log($"Player : {hp.name} Distance : {distance:F2}");

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = hp.transform;
                nearestHealth = hp;
            }
        }

        if (nearestTarget != null)
        {
            target = nearestTarget;
            targetHealth = nearestHealth;

            Debug.Log("Locked Target : " + target.name);
        }
        else
        {
            Debug.Log("No valid player found.");
        }
    }

    void Chase()
    {
        if (!agent.enabled)
        {
            Debug.LogWarning("NavMeshAgent Disabled");
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(target.position);


        if (anim != null)
        {
            anim.SetBool("Walk", true);
            EnemyAudio.Instance.StartFootsteps();
        }
    }

    void Attack()
    {
        agent.isStopped = true;
        if (anim != null)
        {
            anim.SetBool("Walk", false);
            EnemyAudio.Instance.StopFootsteps();
        }
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                10f * Time.deltaTime
            );
        }

        

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0;

            Debug.Log("Zombie attacked " + target.name);

            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
                if (anim != null)
                {
                    anim.SetBool("Walk", false);

                    EnemyAudio.Instance.StopFootsteps();
                    anim.Play("Attack01");
                    CameraShake.shakeCamera = true;
                    EnemyAudio.Instance.PlayAttack();
                    StartCoroutine(playHitEffect());

                }
            }
        }
    }
    public float hitEffectDelay = 0.3f;
    IEnumerator playHitEffect()
    {
        yield return new WaitForSeconds(hitEffectDelay);
        hitEffect.Play();
    }
    public void stopMovement()
    {
        if (agent.enabled)
            agent.isStopped = true;
        agent.speed = 0;
    }
    void Idle()
    {
        if (agent.enabled)
            agent.isStopped = true;

        if (anim != null)
        {
            anim.SetBool("Walk", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
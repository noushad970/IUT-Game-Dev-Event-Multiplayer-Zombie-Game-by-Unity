using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAIMulti : MonoBehaviourPun
{
    [Header("Detection")]
    public LayerMask playerLayer;
    public float detectionRadius = 50f;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;

    [Header("Effects")]
    public ParticleSystem hitEffect;
    public float hitEffectDelay = 0.3f;

    private NavMeshAgent agent;
    private Animator anim;

    private Transform target;
    private    PlayerHealthMulti  targetHealth;
    EnemyAudioMulti enemyAudio;

    private float attackTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyAudio = GetComponent<EnemyAudioMulti>();
        Debug.Log("Enemy Initialized");
    }

    void Update()
    {
        // Only Master controls AI
        if (!PhotonNetwork.IsMasterClient)
            return;

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
            target = null;
            targetHealth = null;
            return;
        }

        float distance =
            Vector3.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            Chase();
        }
        else
        {
            Attack();
        }
    }

    //==================================================
    // SEARCH PLAYER
    //==================================================

    void SearchPlayer()
    {
        Collider[] players = Physics.OverlapSphere(
            transform.position,
            detectionRadius,
            playerLayer);

        float nearestDistance = Mathf.Infinity;

        Transform nearestPlayer = null;
        PlayerHealthMulti nearestHealth = null;

        foreach (Collider col in players)
        {
            PlayerHealthMulti hp =
                col.GetComponent<PlayerHealthMulti>();

            if (hp == null)
                hp = col.GetComponentInParent<PlayerHealthMulti>();

            if (hp == null)
                hp = col.GetComponentInChildren<PlayerHealthMulti>();

            if (hp == null)
                continue;

            if (hp.IsDead)
                continue;

            float dist =
                Vector3.Distance(transform.position,
                hp.transform.position);

            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestPlayer = hp.transform;
                nearestHealth = hp;
            }
        }

        target = nearestPlayer;
        targetHealth = nearestHealth;
    }

    //==================================================
    // CHASE
    //==================================================

    void Chase()
    {
        if (!agent.enabled)
            return;

        agent.isStopped = false;
        agent.SetDestination(target.position);

        if (anim != null)
            anim.SetBool("Walk", true);

        if (enemyAudio != null)
            enemyAudio.StartWalk();
    }

    //==================================================
    // ATTACK
    //==================================================

    void Attack()
    {
        if (agent.enabled)
            agent.isStopped = true;

        if (anim != null)
            anim.SetBool("Walk", false);
        if (enemyAudio != null) enemyAudio.StopWalk();

        Vector3 dir =
            target.position - transform.position;

        dir.y = 0;

        if (dir != Vector3.zero)
        {
            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(dir),
                    10f * Time.deltaTime);
        }

        attackTimer += Time.deltaTime;

        if (attackTimer < attackCooldown)
            return;

        attackTimer = 0;

        if (anim != null)
        {

            anim.SetTrigger("Attack01");
            enemyAudio.PlayAttack();
        }

        photonView.RPC(
            nameof(RPC_PlayHitEffect),
            RpcTarget.All);

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage);
        }
    }

    //==================================================
    // HIT EFFECT
    //==================================================

    [PunRPC]
    void RPC_PlayHitEffect()
    {
        StartCoroutine(PlayHitEffect());
    }

    IEnumerator PlayHitEffect()
    {
        yield return new WaitForSeconds(hitEffectDelay);

        if (hitEffect != null)
            hitEffect.Play();
    }

    //==================================================
    // STOP MOVEMENT
    //==================================================

    public void stopMovement()
    {
        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.speed = 0;
        }
    }

    //==================================================
    // IDLE
    //==================================================

    void Idle()
    {
        if (agent.enabled)
            agent.isStopped = true;

        if (anim != null)
        {
            enemyAudio.StopWalk();
            anim.SetBool("Walk", false);
        }
    }

    //==================================================
    // GIZMOS
    //==================================================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
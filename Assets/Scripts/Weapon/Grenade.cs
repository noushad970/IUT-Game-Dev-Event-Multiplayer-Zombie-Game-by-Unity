using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosion")]
    public float fuseTime = 3f;

    public float explosionRadius = 5f;

    public int damage = 100;

    public LayerMask enemyLayer;

    public ParticleSystem explosionEffect;

    private bool exploded;

    private void Start()
    {
        Invoke(nameof(Explode), fuseTime);
    }

    void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        Debug.Log("Grenade Exploded");

        if (explosionEffect != null)
        {
            ParticleSystem effect = Instantiate(
                explosionEffect,
                transform.position,
                Quaternion.identity
            );

            effect.Play();

            Destroy(
                effect.gameObject,
                effect.main.duration +
                effect.main.startLifetime.constantMax
            );
        }

        Collider[] enemies = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            enemyLayer
        );

        Debug.Log("Enemies Hit : " + enemies.Length);

        foreach (Collider enemy in enemies)
        {
            EnemyHealth health =
                enemy.GetComponentInParent<EnemyHealth>();

            if (health == null)
                health = enemy.GetComponent<EnemyHealth>();

            if (health == null)
                continue;

            health.TakeDamage(damage, null);

            Debug.Log("Damaged : " + health.name);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}
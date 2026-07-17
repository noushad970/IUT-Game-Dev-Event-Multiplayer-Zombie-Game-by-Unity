using UnityEngine;
using Photon.Pun;

public class GrenadeMulti : MonoBehaviourPun
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
        // Only the owner starts the fuse timer
        if (photonView.IsMine)
        {
            Invoke(nameof(Explode), fuseTime);
        }
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        // Trigger explosion on ALL clients
        photonView.RPC("RPC_Explode", RpcTarget.All, transform.position);
    }

    [PunRPC]
    void RPC_Explode(Vector3 explosionPos)
    {
        Debug.Log("Grenade Exploded at " + explosionPos);

        // Spawn Explosion Effect (visible to everyone)
        if (explosionEffect != null)
        {
            ParticleSystem effect = Instantiate(
                explosionEffect,
                explosionPos,
                Quaternion.identity
            );

            effect.Play();

            Destroy(effect.gameObject,
                effect.main.duration + effect.main.startLifetime.constantMax);
        }

        // Apply Damage (Only owner applies damage to prevent double damage)
        if (photonView.IsMine)
        {
            Collider[] enemies = Physics.OverlapSphere(
                explosionPos,
                explosionRadius,
                enemyLayer
            );

            Debug.Log("Enemies Hit : " + enemies.Length);

            foreach (Collider enemy in enemies)
            {
                EnemyHealth health = enemy.GetComponentInParent<EnemyHealth>()
                                  ?? enemy.GetComponent<EnemyHealth>();

                if (health != null)
                {
                    health.TakeDamage(damage, null);
                    Debug.Log("Damaged : " + health.name);
                }
            }

            // Destroy the grenade on all clients
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
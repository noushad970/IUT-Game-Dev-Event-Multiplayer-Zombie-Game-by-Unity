using System.Collections;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    public float speed = 80f;

    [Header("Blood Effect")]
    public ParticleSystem bloodEffect;
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is on the Enemy layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (bloodEffect != null)
            {
                // Detach so it remains after the bullet is destroyed
                bloodEffect.Play();
                Debug.Log("Blood Effect Played");
            }
        }

    }
}
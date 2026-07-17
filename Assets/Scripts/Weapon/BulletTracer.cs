using System.Collections;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    public float speed = 80f;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is on the Enemy layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            
        }

    }
}
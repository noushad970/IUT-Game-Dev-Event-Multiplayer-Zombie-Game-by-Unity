using System;
using UnityEngine;

public class ZombieDeath : MonoBehaviour
{
    public event System.Action<GameObject> OnZombieDeath;

    public void Die()
    {
        OnZombieDeath?.Invoke(gameObject);

        Destroy(gameObject);
    }
}
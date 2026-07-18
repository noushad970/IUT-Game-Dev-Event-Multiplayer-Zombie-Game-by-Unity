using System;
using Photon.Pun;
using UnityEngine;

public class ZombieDeathMulti : MonoBehaviourPun
{
    public event Action<GameObject> OnZombieDeath;

    public void Die()
    {
        OnZombieDeath?.Invoke(gameObject);

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
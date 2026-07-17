using Photon.Pun;
using UnityEngine;

public class PlayerSpawnerMulti : MonoBehaviour
{
    public Transform[] spawnPoints;

    void Start()
    {
        int index = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        index %= spawnPoints.Length;

        string prefab =
            AuthenticationManager.Instance.getSelectedCharacter() == 1
            ? "MalePlayer"
            : "FemalePlayer";

        PhotonNetwork.Instantiate(
            prefab,
            spawnPoints[index].position,
            spawnPoints[index].rotation
        );
    }
}
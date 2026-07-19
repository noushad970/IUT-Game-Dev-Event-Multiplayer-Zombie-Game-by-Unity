using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSpawnerMulti : MonoBehaviourPunCallbacks
{
    [Header("Spawn Points (Assign 4 in Inspector)")]
    public Transform[] spawnPoints;
    public GameObject GameOverPanel;

    private void Start()
    {
        SpawnPlayer();
        GameOverPanel.SetActive(false);
    }

    void SpawnPlayer()
    {
        if (!PhotonNetwork.InRoom)
            return;

        if (spawnPoints.Length < 4)
        {
            Debug.LogError("Please assign 4 spawn points.");
            return;
        }

        //--------------------------------------------------
        // Find this player's index inside the room
        //--------------------------------------------------

        Player[] players = PhotonNetwork.PlayerList;

        int spawnIndex = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == PhotonNetwork.LocalPlayer)
            {
                spawnIndex = i;
                break;
            }
        }

        //--------------------------------------------------
        // Safety
        //--------------------------------------------------

        spawnIndex = Mathf.Clamp(spawnIndex, 0, spawnPoints.Length - 1);

        //--------------------------------------------------
        // Character
        //--------------------------------------------------

        string prefab =
            AuthenticationManager.Instance.getSelectedCharacter() == 1
            ? "MalePlayer"
            : "FemalePlayer";

        //--------------------------------------------------
        // Spawn
        //--------------------------------------------------

        PhotonNetwork.Instantiate(
            prefab,
            spawnPoints[spawnIndex].position,
            spawnPoints[spawnIndex].rotation
        );

        Debug.Log(
            PhotonNetwork.LocalPlayer.NickName +
            " Spawned at Spawn Point : " +
            (spawnIndex + 1));
    }
}
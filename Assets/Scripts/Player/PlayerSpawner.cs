using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject malePlayer, femalePlayer;
    public Transform spawnPoint;
    private async void Start()
    {
        int selected = await AuthenticationManager.Instance.GetSelectedPlayer();

        if (selected == 1)
            Instantiate(malePlayer, spawnPoint.position, spawnPoint.rotation);
        else
            Instantiate(femalePlayer, spawnPoint.position, spawnPoint.rotation);
    }
}

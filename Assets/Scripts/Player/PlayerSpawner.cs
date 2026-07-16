using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject malePlayer, femalePlayer;
    private void Start()
    {
        if(AuthenticationManager.Instance.getSelectedCharacter() == 1)
        {
            malePlayer.SetActive(true);
            femalePlayer.SetActive(false);
        }
        else if (AuthenticationManager.Instance.getSelectedCharacter() == 0)
        {
            malePlayer.SetActive(false);
            femalePlayer.SetActive(true);
        }
    }
}

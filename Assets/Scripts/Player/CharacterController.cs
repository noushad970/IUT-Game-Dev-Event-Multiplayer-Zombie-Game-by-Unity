using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Character Prefabs")]
    public GameObject[] characters;

    public void SetCharacter(int characterID)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == characterID - 1);
        }

        Debug.Log("Character Loaded : " + characterID);
    }
}
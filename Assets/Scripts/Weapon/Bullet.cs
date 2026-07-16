using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet")]
    public int damage = 20;

    // Who fired this bullet
    public GameObject shooter;

    // Optional: store UID for multiplayer/Firebase
    public string shooterUID;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet Hit : " + other.gameObject.name);

        // Ignore the shooter
        if (other.gameObject == shooter)
            return;



        Destroy(gameObject);
    }
}
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;

    public bool IsDead => health <= 0;
    public GameObject player;
    public static bool isPlayerDead = false;
    public static int healthValue = 0;
    private void Start()
    {
        isPlayerDead = false;
        healthValue = health;
    }
    public void TakeDamage(int damage)
    {
        if (IsDead)
            return;

        health -= damage;
        healthValue = health;

        Debug.Log("Player HP : " + health);

        if (health <= 0)
        {
            Debug.Log("Player Died");
            isPlayerDead = true;
            playerDied();
        }
    }
    void playerDied()
    {
        Animator anim=GetComponent<Animator>();
        anim.SetTrigger("Died");
        Destroy(player,3f);
    }
}
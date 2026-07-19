using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;

    [SerializeField]
    public static int currentHealth = 100;

    [Header("UI")]
    public Slider healthSlider;

    public bool IsDead => currentHealth <= 0;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }
    private void Update()
    {
        UpdateHealthUI();
    }
    //--------------------------------------------------
    // DAMAGE
    //--------------------------------------------------

    public void TakeDamage(int damage)
    {
        if (IsDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        Debug.Log(name + " HP : " + currentHealth);

    }

    //--------------------------------------------------
    // HEAL
    //--------------------------------------------------

    public void Heal(int amount)
    {
        if (IsDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
    }

    //--------------------------------------------------
    // SET HEALTH
    //--------------------------------------------------

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    }

    //--------------------------------------------------
    // UI
    //--------------------------------------------------

    void UpdateHealthUI()
    {
        if (healthSlider == null)
            return;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    //--------------------------------------------------
    // DEATH
    //--------------------------------------------------

    

    //--------------------------------------------------
    // GETTERS
    //--------------------------------------------------

    public int GetHealth()
    {
        return currentHealth;
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }
}
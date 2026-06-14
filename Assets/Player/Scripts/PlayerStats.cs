using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 150.0f;
    public float stamina;

    [Header("Health Settings (Quest)")]
    public float maxHp = 100.0f;
    public float currentHp;

    void Start()
    {
        stamina = maxStamina;
        currentHp = maxHp;
    }

    public void UseStamina(float amount)
    {
        stamina -= amount;
        if (stamina < 0) stamina = 0;
    }

    public void RecoverStamina(float amount)
    {
        if (stamina < maxStamina)
        {
            stamina += amount;
            if (stamina > maxStamina) stamina = maxStamina;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        Debug.Log($"플레이어 피격! 현재 체력: {currentHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHp += amount;
        if (currentHp > maxHp) currentHp = maxHp;
        Debug.Log($"플레이어 회복! 현재 체력: {currentHp}");
    }

    private void Die()
    {
        Debug.Log("Player Dead!");
    }
}

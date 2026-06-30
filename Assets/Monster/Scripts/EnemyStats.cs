using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHp = 50f;
    public float currentHp;

    void Start()
    {
        currentHp = maxHp;
    }
    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        Debug.Log($"적 피격! 데미지: {damage}, 남은 체력: {currentHp}");

        if (UIManager.Instance != null)
        {
            UIManager.Instance.OpenEnemyHPBar(this, "Monster");
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("적 처치 완료!");
        Destroy(gameObject);
    }
}

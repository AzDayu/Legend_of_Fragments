using UnityEngine;

public class HealItem : MonoBehaviour
{
    [Header("회복 설정")]
    [Tooltip("아이템 획득 시 회복할 체력의 양")]
    public float healAmount = 20f;

    [Tooltip("아이템 획득 시 나타낼 파티클 이펙트 (선택 사항)")]
    public GameObject healEffectPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.Heal(healAmount);

                if (healEffectPrefab != null)
                {
                    Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }
}

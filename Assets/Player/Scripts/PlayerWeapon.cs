using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public float weaponDamage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(weaponDamage);
            }
        }
    }
}
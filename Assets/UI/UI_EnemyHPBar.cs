using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyHPBar : UIBase
{
    [Header("UI Components")]
    [SerializeField] private Slider hpSlider;

    [Header("Tracking Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.0f, 0f);

    private EnemyStats targetEnemy;
    private Camera mainCamera;

    public override void Setup()
    {
        base.Setup();
        mainCamera = Camera.main;
    }

    public void SetTarget(EnemyStats enemy, string name = "Monster")
    {
        targetEnemy = enemy;
        UpdateHPBar();
    }

    private void Update()
    {
        if (targetEnemy == null)
        {
            gameObject.SetActive(false);
            return;
        }

        UpdateHPBar();
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (mainCamera == null) return;

        Vector3 worldPosition = targetEnemy.transform.position + offset;

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z < 0)
        {
            hpSlider.gameObject.SetActive(false);
        }
        else
        {
            hpSlider.gameObject.SetActive(true);
            transform.position = screenPosition;
        }
    }

    private void UpdateHPBar()
    {
        if (hpSlider == null || targetEnemy == null) return;

        float hpRatio = targetEnemy.currentHp / targetEnemy.maxHp;
        hpSlider.value = hpRatio;
    }
}

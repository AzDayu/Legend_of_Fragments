using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHUD : UIBase
{
    [Header("UI Components")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider staminaSlider;

    private PlayerStats playerStats;

    public override void Setup()
    {
        base.Setup();

        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }

        if (playerStats == null)
        {
            Debug.LogError("[UI_PlayerHUD] 씬에서 PlayerStats를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        if (playerStats == null) return;

        UpdateHPBar();
        UpdateStaminaBar();
    }

    private void UpdateHPBar()
    {
        if (hpSlider == null) return;

        float hpRatio = playerStats.currentHp / playerStats.maxHp;
        hpSlider.value = hpRatio;
    }

    private void UpdateStaminaBar()
    {
        if (staminaSlider == null) return;

        float staminaRatio = playerStats.stamina / playerStats.maxStamina;
        staminaSlider.value = staminaRatio;
    }
}

using System.Collections;
using UnityEngine;

public class EffectZone : MonoBehaviour
{
    [Header("효과 설정")]
    public EffectType effectType;
    public float effectAmount = 10f;
    public float effectInterval = 1f;

    [Header("시각적 구분 피드백")]
    public ParticleSystem zoneParticle;
    public Color activeColor = Color.red;

    private Renderer zoneRenderer;
    private Color originalColor;
    private Coroutine effectCoroutine;

    public enum EffectType
    {
        Damage,
        Heal
    }

    private void Start()
    {
        zoneRenderer = GetComponent<Renderer>();
        if (zoneRenderer != null)
        {
            originalColor = zoneRenderer.material.color;
        }

        if (zoneParticle != null)
        {
            zoneParticle.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                effectCoroutine = StartCoroutine(ApplyEffectRoutine(playerStats));

                Debug.Log($"[EffectZone] 구역 진입! {effectType} 효과가 적용됩니다.");
                if (zoneParticle != null) zoneParticle.Play();
                if (zoneRenderer != null) zoneRenderer.material.color = activeColor;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (effectCoroutine != null)
            {
                StopCoroutine(effectCoroutine);
                effectCoroutine = null;
            }

            Debug.Log("[EffectZone] 구역 이탈! 효과가 중단되었습니다.");
            if (zoneParticle != null) zoneParticle.Stop();
            if (zoneRenderer != null) zoneRenderer.material.color = originalColor;
        }
    }

    private IEnumerator ApplyEffectRoutine(PlayerStats playerStats)
    {
        while (true)
        {
            if (effectType == EffectType.Damage)
            {
                playerStats.TakeDamage(effectAmount);
            }
            else if (effectType == EffectType.Heal)
            {
                playerStats.Heal(effectAmount);
            }

            yield return new WaitForSeconds(effectInterval);
        }
    }
}

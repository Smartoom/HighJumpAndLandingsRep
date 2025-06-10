using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PlayerTeamHandling))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;
    [Header("Health UI Color")]
    [SerializeField] private Color lowHealthUIColor;
    [SerializeField] private Color normalHealthUIColor;
    [SerializeField] private float healthGradientMinFrequency;
    [SerializeField] private float healthGradientMinHealthRatio;
    [SerializeField] private float healthGradientMaxFrequency;
    [SerializeField] private float healthGradientMaxHealthRatio;
    [SerializeField] private float healthGradientColorIntensityMax = 80;
    [SerializeField] private float healthGradientColorIntensityMin = 62;
    [Header("Vignette Effect")]
    [SerializeField] private Color lowHealthVignetteColor;
    [SerializeField] private Color normalHealthVignetteColor;
    [SerializeField] private float vignetteColorChangeSpeed;
    [SerializeField] private float lowHealthVignetteIntensity;
    [SerializeField] private float fullHealthVignetteIntensity;
    [SerializeField] private float healthIntensityChangeSpeed;
    private Volume volume;
    private Vignette vignette;

    private void Start()
    {
        volume = GameReferenceManager.instance.volume;
        volume.profile.TryGet(out vignette);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        vignette.color.Override(lowHealthVignetteColor);
        vignette.intensity.Override(lowHealthVignetteIntensity);

        if (health <= 0)
            Die();
    }
    private void Die()
    {
        PlayerRespawnManager.instance.StartRespawnTimer(GetComponent<PlayerTeamHandling>().teamInt);
        Destroy(transform.parent.gameObject);//assuming the player and the camera are still children of the container.
    }

    private void Update()
    {
        ChangeHealthHUD();
    }

    private void ChangeHealthHUD()
    {
        float healthRatio = (float)health / maxHealth;

        Color healthUIColor = Color.Lerp(lowHealthUIColor, normalHealthUIColor, healthRatio);
        CanvasReferenceManager.instance.healthText.color = healthUIColor;
        CanvasReferenceManager.instance.healthFill.color = healthUIColor;

        CanvasReferenceManager.instance.healthText.text = health.ToString();
        CanvasReferenceManager.instance.healthFill.fillAmount = healthRatio;

        float healthVignetteIntensity = Mathf.Lerp(lowHealthVignetteIntensity, fullHealthVignetteIntensity, (healthRatio));
        float imidiateVignetteIntensity = Mathf.MoveTowards(vignette.intensity.value, healthVignetteIntensity, healthIntensityChangeSpeed * Time.deltaTime);
        vignette.intensity.Override(imidiateVignetteIntensity);

        Color healthVignetteColor = Color.Lerp(lowHealthVignetteColor, normalHealthVignetteColor, (healthRatio));
        Color imidiateVignetteColor = Color.Lerp(vignette.color.value, lowHealthVignetteColor, vignetteColorChangeSpeed * Time.deltaTime);
        vignette.color.Override(imidiateVignetteColor);

        float throbbFrequency;
        if (health >= maxHealth * healthGradientMaxHealthRatio)
        {
            throbbFrequency = healthGradientMinFrequency;
        }
        else if (health > healthGradientMinHealthRatio * maxHealth)
        {
            throbbFrequency = Mathf.Lerp(healthGradientMinFrequency, healthGradientMaxFrequency, 1 - healthRatio);
        }
        else
        {
            throbbFrequency = healthGradientMaxFrequency;
        }
        float amplitude = (healthGradientColorIntensityMax - healthGradientColorIntensityMin) * 0.5f;
        float throbbing = Mathf.Sin(Time.time * throbbFrequency) * amplitude;
        CanvasReferenceManager.instance.healthBackgroundPainCanvasGroup.alpha = throbbing * (1 - healthRatio) + healthRatio;
    }

    [ContextMenu("Remove 10 helth")]
    private void Remove10HealthContextFunction()
    {
        TakeDamage(10);
    }
}

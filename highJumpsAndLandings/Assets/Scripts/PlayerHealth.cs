using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;
    [Header("Health UI Color")]
    [SerializeField] private Color lowHealthUIColor;
    [SerializeField] private Color normalHealthUIColor;
    [Header("Vignette Effect")]
    [SerializeField] private Color lowHealthVignetteColor;
    [SerializeField] private Color normalHealthVignetteColor;
    [SerializeField] private float healthColorChangeSpeed;
    [SerializeField] private float lowHealthVignetteIntensity;
    [SerializeField] private float fullHealthVignetteIntensity;
    [SerializeField] private float healthIntensityChangeSpeed;
    [SerializeField] private Volume volume;
    private Vignette vignette;

    private bool gameOver = false;

    private void Start()
    {
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
        CanvasReferenceManager.instance.gameHUDScreen.SetActive(false);
        CanvasReferenceManager.instance.gameOverScreen.SetActive(true);
        gameOver = true;
        Time.timeScale = 0;
    }
    private void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
        Color healthUIColor = Color.Lerp(lowHealthUIColor, normalHealthUIColor, ((float)health / maxHealth));
        CanvasReferenceManager.instance.healthText.color = healthUIColor;
        CanvasReferenceManager.instance.healthFill.color = healthUIColor;

        CanvasReferenceManager.instance.healthText.text = health.ToString();
        CanvasReferenceManager.instance.healthFill.fillAmount = (float)health / maxHealth;

        float healthVignetteIntensity = Mathf.Lerp(lowHealthVignetteIntensity, fullHealthVignetteIntensity, ((float)health / maxHealth));
        float imidiateVignetteIntensity = Mathf.MoveTowards(vignette.intensity.value, healthVignetteIntensity, healthIntensityChangeSpeed * Time.deltaTime);
        vignette.intensity.Override(imidiateVignetteIntensity);

        Color healthVignetteColor = Color.Lerp(lowHealthVignetteColor, normalHealthVignetteColor, ((float)health / maxHealth));
        Color imidiateVignetteColor = Color.Lerp(vignette.color.value, lowHealthVignetteColor, healthColorChangeSpeed * Time.deltaTime);
        vignette.color.Override(imidiateVignetteColor);
    }
}

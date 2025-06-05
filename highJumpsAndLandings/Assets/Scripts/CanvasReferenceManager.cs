using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasReferenceManager : MonoBehaviour
{
    public static CanvasReferenceManager instance;
    public GameObject gameHUDScreen;
    public GameObject gameOverScreen;
    public TMP_Text healthText;
    public Image healthFill;
    public Image healthBackgroundImage;
    public CanvasGroup healthBackgroundPainCanvasGroup;
    // CROSSHAIRS
    public Transform plusCrosshair;
    public RectTransform[] plusCrosshairBars;
    public Transform xCrosshair;
    public Transform circleCrosshair;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("!!!!!!!!!!!!!!!!######!!!!!!!!!!!!");
            return;
        }
        instance = this;
    }
    public void DeactivateCrosshairs()
    {
        plusCrosshair.gameObject.SetActive(false);
        xCrosshair.gameObject.SetActive(false);
        circleCrosshair.gameObject.SetActive(false);
    }
}

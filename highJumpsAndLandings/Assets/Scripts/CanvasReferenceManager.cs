using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasReferenceManager : MonoBehaviour
{
    public static CanvasReferenceManager instance;
    [Header("SCREEN SWITCHING")]
    public GameObject gameHUDScreen;
    public GameObject gameOverScreen;
    [Header("HEALTH")]
    public TMP_Text healthText;
    public Image healthFill;
    public Image healthBackgroundImage;
    public CanvasGroup healthBackgroundPainCanvasGroup;
    [Header("PLAYER TEAM INDICATOR")]
    public Image playerTeamImage;
    [Header("RESPAWNING")]
    public TMP_Text respawnTimerText;
    [Header("TEAM CHART")]
    public GameObject teamUIChart;
    public Transform redTeamFill;
    public Transform blueTeamFill;
    public Transform yellowTeamFill;
    public Transform purpleTeamFill;
    public Transform greenTeamFill;
    public TMP_Text redTeamNumberText;
    public TMP_Text blueTeamNumberText;
    public TMP_Text yellowTeamNumberText;
    public TMP_Text purpleTeamNumberText;
    public TMP_Text greenTeamNumberText;
    [Header("CROSSHAIR")]
    // CROSSHAIRS
    public Transform plusCrosshair;
    //public RectTransform[] plusCrosshairBars;
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

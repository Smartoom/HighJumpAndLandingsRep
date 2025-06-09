using UnityEngine;

public class PlayerRespawnManager : MonoBehaviour
{
    public static PlayerRespawnManager instance;
    [SerializeField] private GameObject playerContainerPrefab;
    [SerializeField] private GameObject respawnCamera;
    [SerializeField] private float timeToRespawn = 5;
    [SerializeField] private Sprite noTeamSprite;
    float timer = 0;
    private bool wantsToRespawn = false;
    private int playerTeamWhenDead;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("shit ass");
            return;
        }
        instance = this;
    }
    private void Update()
    {
        if (wantsToRespawn)
        {
            CanvasReferenceManager.instance.respawnTimerText.text = ((int)timer).ToString();
            timer += Time.deltaTime;
            if (timer >= timeToRespawn)
            {
                timer = 0;
                wantsToRespawn = false;
                Instantiate(playerContainerPrefab, Vector3.zero, Quaternion.identity).GetComponentInChildren<PlayerTeamHandling>().SetTeamInt(playerTeamWhenDead);

                respawnCamera.SetActive(false);
                if (playerTeamWhenDead < 0)
                    CanvasReferenceManager.instance.playerTeamImage.sprite = noTeamSprite;
            }
        }
    }

    public void StartRespawnTimer(int playerTeamInt)
    {
        CanvasReferenceManager.instance.gameHUDScreen.SetActive(false);
        CanvasReferenceManager.instance.gameOverScreen.SetActive(true);
        playerTeamWhenDead = playerTeamInt;
        wantsToRespawn = true;
        respawnCamera.SetActive(true);
    }
}

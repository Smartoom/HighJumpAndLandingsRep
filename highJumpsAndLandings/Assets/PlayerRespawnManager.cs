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
            CanvasReferenceManager.instance.respawnTimerText.text = ((int)(timeToRespawn - timer + 1)).ToString();
            timer += Time.deltaTime;
            if (timer >= timeToRespawn)
            {
                timer = 0;
                wantsToRespawn = false;
                GameObject playerInst = Instantiate(playerContainerPrefab, Vector3.zero, Quaternion.identity);
                GameReferenceManager.instance.player = playerInst.transform;
                GameReferenceManager.instance.playerTeamHandling = playerInst.GetComponentInChildren<PlayerTeamHandling>();
                GameReferenceManager.instance.playerTeamHandling.SetTeamInt(playerTeamWhenDead);

                CanvasReferenceManager.instance.gameHUDScreen.SetActive(true);
                CanvasReferenceManager.instance.deathScreen.SetActive(false);
                respawnCamera.SetActive(false);

                if (playerTeamWhenDead < 0)
                    CanvasReferenceManager.instance.playerTeamImage.sprite = noTeamSprite;
            }
        }
    }

    public void StartRespawnTimer(int playerTeamInt)
    {
        CanvasReferenceManager.instance.gameHUDScreen.SetActive(false);
        CanvasReferenceManager.instance.deathScreen.SetActive(true);
        MiniMapCamera.instance.SetFollowTarget(null);
        playerTeamWhenDead = playerTeamInt;
        wantsToRespawn = true;
        respawnCamera.SetActive(true);
    }
}

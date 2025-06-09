using UnityEngine;
using UnityEngine.Rendering;

public class GameReferenceManager : MonoBehaviour
{
    public static GameReferenceManager instance;
    /// <summary>
    /// only use in playerHealth
    /// </summary>
    public Volume volume;
    public Transform player;
    public PlayerTeamHandling playerTeamHandling;

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
}

using UnityEngine;

public class GameReferenceManager : MonoBehaviour
{
    public static GameReferenceManager instance;
    public Transform player;

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

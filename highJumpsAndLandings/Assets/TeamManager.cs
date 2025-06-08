using UnityEngine;

public class TeamManager : MonoBehaviour
{

    [System.Serializable]
    public class Team
    {
        public string teamName;
        public Material identificationAccesoryMaterial;
        public Sprite teamIcon;
    }
    public Team[] teams;

    public static TeamManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("sigma");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
}

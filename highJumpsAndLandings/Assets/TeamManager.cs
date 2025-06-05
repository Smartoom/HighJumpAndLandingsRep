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
}

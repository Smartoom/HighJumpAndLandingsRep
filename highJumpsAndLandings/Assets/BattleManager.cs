using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public List<TeamedCharacter> teamedCharactersInScene;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("ligma detected");
            Destroy(gameObject);
            return;
        }
        instance = this;
        teamedCharactersInScene = new List<TeamedCharacter>();
    }
}

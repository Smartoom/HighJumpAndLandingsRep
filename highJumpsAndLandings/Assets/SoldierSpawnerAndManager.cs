using UnityEngine;

public class SoldierSpawnerAndManager : MonoBehaviour
{
    [SerializeField] private GenericSoldierDudeEnemyScript soldierPrefab;
    [SerializeField] private Transform SoldierContainer;
    [SerializeField] private int maxSoldierCount;
    [SerializeField] private float timeToRespawnSoldier;
    [SerializeField] private Vector3 mapArenaRange;
    [SerializeField] private float soldierInTeamRandomDistance = 3f;
    int[] teamSoldierCount;
    float[] teamRespawnTimer;

    private void Start()
    {
        teamRespawnTimer = new float[TeamManager.instance.teams.Length];

        //spawn soldiers
        for (int i = 0; i < TeamManager.instance.teams.Length; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-mapArenaRange.x / 2, mapArenaRange.x / 2), 0.5f, Random.Range(-mapArenaRange.z / 2, mapArenaRange.z / 2));
            for (int j = 0; j < maxSoldierCount; j++)
            {
                Vector3 randomDifference = new Vector3(Random.Range(-soldierInTeamRandomDistance, soldierInTeamRandomDistance), 0, Random.Range(-soldierInTeamRandomDistance, soldierInTeamRandomDistance));
                GenericSoldierDudeEnemyScript soldierScript = Instantiate(soldierPrefab, spawnPos + randomDifference, Quaternion.identity, SoldierContainer);
                soldierScript.SetTeamInt(i);
                if (j == 0)
                    soldierScript.SetAsCommanderContextMenu();
            }
        }
    }
    private void Update()
    {
        IdentifySoldierCount();
        CanvasReferenceManager.instance.redTeamFill.localScale = new Vector3((teamSoldierCount[0]) / (float)maxSoldierCount, 1, 1);
        CanvasReferenceManager.instance.redTeamNumberText.text = teamSoldierCount[0].ToString();
        CanvasReferenceManager.instance.blueTeamFill.localScale = new Vector3((teamSoldierCount[1]) / (float)maxSoldierCount, 1, 1);
        CanvasReferenceManager.instance.blueTeamNumberText.text = teamSoldierCount[1].ToString();
        CanvasReferenceManager.instance.yellowTeamFill.localScale = new Vector3((teamSoldierCount[2]) / (float)maxSoldierCount, 1, 1);
        CanvasReferenceManager.instance.yellowTeamNumberText.text = teamSoldierCount[2].ToString();
        CanvasReferenceManager.instance.purpleTeamFill.localScale = new Vector3((teamSoldierCount[3]) / (float)maxSoldierCount, 1, 1);
        CanvasReferenceManager.instance.purpleTeamNumberText.text = teamSoldierCount[3].ToString();
        CanvasReferenceManager.instance.greenTeamFill.localScale = new Vector3((teamSoldierCount[4]) / (float)maxSoldierCount, 1, 1);
        CanvasReferenceManager.instance.greenTeamNumberText.text = teamSoldierCount[4].ToString();
        for (int i = 0; i < teamSoldierCount.Length; i++)
        {
            if (teamSoldierCount[i] == 0)
            {
                //GAME OVER. or not
            }
            else if (teamSoldierCount[i] < maxSoldierCount)
            {
                teamRespawnTimer[i] += Time.deltaTime;
                if (teamRespawnTimer[i] >= timeToRespawnSoldier)
                {
                    SpawnSoldier(i);
                    teamRespawnTimer[i] = 0;
                }
            }
        }
    }
    private void SpawnSoldier(int soldierTeamInt)
    {
        Debug.Log("spawn soldier for " + TeamManager.instance.teams[soldierTeamInt].teamName + " team");

        Vector3 spawnPos = Vector3.up * 0.5f;
        foreach (TeamedCharacter soldier in BattleManager.instance.teamedCharactersInScene)
        {
            if (soldier is PlayerTeamHandling)
                continue;

            if (soldier.teamInt == soldierTeamInt)
                spawnPos = soldier.transform.position;
            if (soldier is GenericSoldierDudeEnemyScript script && script.isSoldierCommander())
                break;
        }
        Instantiate(soldierPrefab, spawnPos, Quaternion.identity, SoldierContainer).SetTeamInt(soldierTeamInt);
    }
    private void IdentifySoldierCount()
    {
        teamSoldierCount = new int[TeamManager.instance.teams.Length];
        //identify enemies and allies (including possibly player)
        foreach (TeamedCharacter soldier in BattleManager.instance.teamedCharactersInScene)
        {
            if (soldier is PlayerTeamHandling)
                continue;

            teamSoldierCount[soldier.teamInt]++;
        }
    }

    private void FixedUpdate()
    {

    }
}

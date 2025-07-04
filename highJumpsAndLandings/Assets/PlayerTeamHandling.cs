using UnityEngine;

public class PlayerTeamHandling : TeamedCharacter
{
    [SerializeField] private KeyCode joinSoldierTeam;
    [SerializeField] private Transform cam;
    [SerializeField] private LayerMask soldierLayers;

    private void Start()
    {
        BattleManager.instance.teamedCharactersInScene.Add(this);
    }
    void Update()
    {
        PickingTeams();
    }
    private void PickingTeams()
    {
        if (!Input.GetKeyDown(joinSoldierTeam))
            return;
        if (!Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 200, soldierLayers))
            return;

        Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
        if (!enemy)
            return;

        SwitchToTeam(enemy.teamInt);
    }
    private void SwitchToTeam(int teamToSwitchTo)
    {
        if (teamInt == teamToSwitchTo)
            return;

        teamInt = teamToSwitchTo;
        if (teamToSwitchTo >= 0)
        {
            CanvasReferenceManager.instance.playerTeamImage.sprite = TeamManager.instance.teams[teamInt].teamIcon;
        }
    }
    private void OnDestroy()
    {
        BattleManager.instance.teamedCharactersInScene.Remove(this);
    }
    /// <summary>
    /// used by spawner of player
    /// </summary>
    public void SetTeamInt(int spawnedTeamInt)
    {
        SwitchToTeam(spawnedTeamInt);
    }
}

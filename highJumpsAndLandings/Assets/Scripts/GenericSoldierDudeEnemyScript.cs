using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenericSoldierDudeEnemyScript : Enemy
{
    [Header("State Management")]
    [SerializeField] private SoldierState soldierState;
    private enum SoldierState
    {
        CommandOrObide,
        ShootInSpot,//chance to be ShootInSpot or ShootWhileRunning
        ShootWhileRunning,
        RunForCover,//when arrived.go to reload
        Reloading,
        SeekingOut,//chance of seeking out the guy where they last seen him or holding the angle.
        Holding// random time of deciding to seek out.
    }
    [Header("Comanding")]
    [SerializeField] private MeshRenderer commanderIndicatorMesh;
    [SerializeField] private int numberOfRandomAttemptsToFindCommandDestinationSpot = 10;
    [SerializeField] private int distanceCloseEnoughToDestinationPoint = 3;
    [SerializeField] private int maxRandomRangeOfDestination = 20;
    private bool isCommander;
    [Header("Remembering")]
    private Vector3 rememberedChosenThreatPosition;
    /*    private Vector3 positionAtWhichThreatWasLastSeen;// :D*/
    [Header("Spotting")]
    [SerializeField] private float fieldOfViewAngle;//sight angle
    [SerializeField] private float viewDistance;//sight angle
    [Header("Attack")]
    [SerializeField] private float shotsPerMinute;// fire rate
    [SerializeField] private float rotateTowardThreatSpeed;// fire rate
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private int magazineSize;
    private int bulletsLoaded;
    private float timeSinceLastShot;
    private bool reloading = false;
    [Header("Attack while running")]
    [SerializeField] private float runningDestinationMinimumDistance = 3;
    [SerializeField] private float runningDestinationMaximumDistance = 7;
    [Header("Hiding")]
    [SerializeField] private int numberOfRandomAttemptsToFindHidingSpot = 10;
    [SerializeField] private int numberOfRandomAttemptsToFindRunningDestination = 10;
    [SerializeField] private int distAwayHideCheck = 10;
    [Header("Blood Particles")]
    [SerializeField] private ParticleSystem bloodParticles;
    [SerializeField] private Transform bloodParticleCollisionPlane;
    [SerializeField] private LayerMask groundLayerMask;
    //reaction time? #todo
    [Header("Other")]
    [SerializeField] private Animator animator;
    [SerializeField] private MeshRenderer armEmblemWrap;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private void Start()
    {
        BattleManager.instance.teamedCharactersInScene.Add(this);
        bulletsLoaded = magazineSize;

        possibleThreats = new();
        allies = new();
        threatsInVision = new();

        armEmblemWrap.material = TeamManager.instance.teams[teamInt].identificationAccesoryMaterial;
    }
    private bool SameTeamAs(int otherTeamInt) => otherTeamInt == teamInt;

    Transform lookAtTransform = null;
    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (lookAtTransform)//may cause bugs later
        {
            Vector3 direction = new Vector3(lookAtTransform.position.x, transform.position.y, lookAtTransform.position.z) - transform.position;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), rotateTowardThreatSpeed * Time.deltaTime);
        }
        animator.SetBool("Running", navMeshAgent.remainingDistance != 0);
    }
    TeamedCharacter chosenCharacter;
    private Vector3 commandGivenDestination;
    private void SetCommandGivenDestination(Vector3 newTeamDestination)
    {
        commandGivenDestination = newTeamDestination;
        if (soldierState == SoldierState.CommandOrObide)
        {
            navMeshAgent.SetDestination(commandGivenDestination);
        }
    }
    private void FixedUpdate()
    {
        switch (soldierState)
        {
            case SoldierState.CommandOrObide:
                IdentifySoldiers();
                //Find a valid random position to go to.
                //with allies, run close to that point.
                //once there do it again.
                if (isCommander)
                {
                    if (navMeshAgent.remainingDistance < distanceCloseEnoughToDestinationPoint)
                    {
                        //update
                        Vector3 foundDestination = transform.position;
                        for (int i = 0; i < numberOfRandomAttemptsToFindCommandDestinationSpot; i++)
                        {
                            float randAngle = Random.Range(0, Mathf.PI * 2);
                            Vector3 possibleDestination = transform.position + new Vector3(Mathf.Cos(randAngle), 0, Mathf.Sin(randAngle)) * Random.Range(0, maxRandomRangeOfDestination);
                            //maybe also check if it is possible to go there with navmesh.
                            if (Physics.Raycast(possibleDestination, Vector3.down))
                            {
                                foundDestination = possibleDestination;
                                break;
                            }
                        }
                        navMeshAgent.SetDestination(foundDestination);
                        foreach (TeamedCharacter allyCharacter in allies)
                        {
                            if (allyCharacter is PlayerTeamHandling)
                                continue;
                            //allies excluding player
                            GenericSoldierDudeEnemyScript ally = (GenericSoldierDudeEnemyScript)allyCharacter;
                            //assuming only one commander exists
                            //Debug.Log("GAAH! only one commander is supposed to exist!");
                            //tell ally where to go
                            ally.SetCommandGivenDestination(foundDestination + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)));// #idk what to do w this bruh. gotta design this stuff.
                        }
                    }
                    //maybe hvae this on an interval

                }
                if (threatsInVision.Count > 0)
                {
                    if (threatsInVision.Contains(GameReferenceManager.instance.playerTeamHandling))//player in vision and is enemy.
                    {
                        chosenCharacter = GameReferenceManager.instance.playerTeamHandling;//prioritize fighting player
                    }
                    else
                        chosenCharacter = threatsInVision[0];//if not fight the first soldier in vision

                    rememberedChosenThreatPosition = chosenCharacter.transform.position;

                    if (Random.value > 0.5)//random choice
                    {
                        //Debug.Log("I see someone, i'l stand and shoot");
                        soldierState = SoldierState.ShootInSpot;
                    }
                    else
                    {
                        //Debug.Log("I see someone, i'l run and gun");
                        GoToRandomRunningShootingPosition();
                        soldierState = SoldierState.ShootWhileRunning;
                    }
                }
                break;
            case SoldierState.ShootInSpot:
                navMeshAgent.SetDestination(transform.position);
                if (chosenCharacter == null)//enemy is deleted/dead
                {
                    //Debug.Log("bruh. he died");
                    soldierState = SoldierState.CommandOrObide;//idk. maybe replace with run for cover or smth
                    break;
                }
                //still have vision of threat?
                bool hitInThreatDir = Physics.Raycast(transform.position, chosenCharacter.transform.position - transform.position, out RaycastHit visionHit, viewDistance);
                if (hitInThreatDir == false || visionHit.collider.transform.parent != chosenCharacter.transform)
                {
                    //Debug.Log("can't see ya. seeking you out.");
                    navMeshAgent.SetDestination(rememberedChosenThreatPosition);
                    soldierState = SoldierState.SeekingOut;
                    break;
                }
                lookAtTransform = chosenCharacter.transform;
                //positionAtWhichThreatWasLastSeen = transform.position;//for finding enemy after reload
                rememberedChosenThreatPosition = chosenCharacter.transform.position;
                if (!HasBullets())
                {
                    //Debug.Log("I need more BOOLETS! run for cover! ...");
                    navMeshAgent.SetDestination(FindCoverPosition(chosenCharacter.transform.position));
                    soldierState = SoldierState.RunForCover;//find a spot?
                    break;
                }
                if (ShootIntervalPassed())
                {
                    GameObject instBullet = Instantiate(bullet, bulletSpawnPosition.position, Quaternion.identity);
                    instBullet.transform.LookAt(chosenCharacter.transform);//replace
                    timeSinceLastShot = 0;
                    bulletsLoaded--;
                }
                break;
            case SoldierState.ShootWhileRunning:

                if (chosenCharacter == null)//enemy is deleted/dead
                {
                    //Debug.Log("bruh. he died while i was running and shooting");
                    soldierState = SoldierState.CommandOrObide;//idk. maybe replace with run for cover or smth
                    break;
                }
                //still have vision of threat?
                bool hitInThreatDirRun = Physics.Raycast(transform.position, chosenCharacter.transform.position - transform.position, out RaycastHit visionHitRun, viewDistance);
                if (hitInThreatDirRun == false || visionHitRun.collider.transform.parent != chosenCharacter.transform)
                {
                    //Debug.Log("can't see ya. seeking you out.");
                    navMeshAgent.SetDestination(rememberedChosenThreatPosition);
                    soldierState = SoldierState.SeekingOut;
                    break;
                }
                lookAtTransform = chosenCharacter.transform;
                //positionAtWhichThreatWasLastSeen = transform.position;//for finding enemy after reload
                rememberedChosenThreatPosition = chosenCharacter.transform.position;
                if (!HasBullets())
                {
                    //Debug.Log("I need more BOOLETS! run for cover! ...");
                    navMeshAgent.SetDestination(FindCoverPosition(chosenCharacter.transform.position));
                    soldierState = SoldierState.RunForCover;//find a spot?
                    break;
                }
                if (ShootIntervalPassed())
                {
                    GameObject instBullet = Instantiate(bullet, bulletSpawnPosition.position, Quaternion.identity);
                    instBullet.transform.LookAt(chosenCharacter.transform);//replace
                    timeSinceLastShot = 0;
                    bulletsLoaded--;
                }
                break;
            case SoldierState.RunForCover:
                if (navMeshAgent.remainingDistance < 1)
                {
                    //once there. if no bullets. reload.
                    Reload();
                    //Debug.Log("i'm close to cover, time to relaod");
                    soldierState = SoldierState.Reloading;//maybe make more behaviour. if you care
                }
                //if you see an enemy. have a chance to run for cover again. ##maybe do later. todo
                break;
            case SoldierState.Reloading:
                if (!reloading)//finisehd reloading
                {
                    //Debug.Log("reloaded, seeking out where i last seen an enemy");
                    navMeshAgent.SetDestination(rememberedChosenThreatPosition);
                    soldierState = SoldierState.SeekingOut;//or hold angle. ##todo
                }
                break;
            case SoldierState.SeekingOut:
                IdentifySoldiers();
                if (threatsInVision.Count > 0)
                {
                    //Debug.Log("I was seeking out an enemy I saw, and now I see an enemy!");
                    if (threatsInVision.Contains(GameReferenceManager.instance.playerTeamHandling))//player in vision and is enemy.
                    {
                        chosenCharacter = GameReferenceManager.instance.playerTeamHandling;//prioritize fighting player
                    }
                    else
                        chosenCharacter = threatsInVision[0];//if not fight the first soldier in vision

                    rememberedChosenThreatPosition = chosenCharacter.transform.position;
                    if (Random.value > 0.5)//random choice
                    {
                        soldierState = SoldierState.ShootInSpot;
                    }
                    else
                    {
                        GoToRandomRunningShootingPosition();
                        soldierState = SoldierState.ShootWhileRunning;
                    }
                }
                else if (navMeshAgent.remainingDistance == 0)
                {
                    //Debug.Log("Couldn't find shit here. going back to commanding or obiding");
                    soldierState = SoldierState.CommandOrObide;
                }
                break;
            case SoldierState.Holding://eh
                break;
            default:
                break;
        }
    }
    private void GoToRandomRunningShootingPosition()
    {
        Vector3 foundRunningDestination = transform.position;//incase none are found
        for (int i = 0; i < numberOfRandomAttemptsToFindRunningDestination; i++)
        {
            float randAngle = Random.Range(0, Mathf.PI * 2);
            Vector3 possibleDestination = chosenCharacter.transform.position + new Vector3(Mathf.Cos(randAngle), 0, Mathf.Sin(randAngle)) * Random.Range(runningDestinationMinimumDistance, runningDestinationMaximumDistance);
            //maybe also check if it is possible to go there with navmesh.
            if (Physics.Raycast(possibleDestination, chosenCharacter.transform.position - possibleDestination, out RaycastHit runningShootingHit) && runningShootingHit.collider.transform.parent == chosenCharacter.transform)
            {
                foundRunningDestination = possibleDestination;
                break;
            }
        }
        navMeshAgent.SetDestination(foundRunningDestination);
    }

    Vector3[] possibleHidingSpots = new Vector3[0];
    int chosenHidingSpot = 0;//for debugging
    private Vector3 FindCoverPosition(Vector3 threatPos) // :( :)
    {
        possibleHidingSpots = new Vector3[numberOfRandomAttemptsToFindHidingSpot];
        for (int i = 0; i < numberOfRandomAttemptsToFindHidingSpot; i++)
        {
            float randAngle = Random.Range(0, Mathf.PI * 2);
            possibleHidingSpots[i] = transform.position + new Vector3(Mathf.Cos(randAngle), 0, Mathf.Sin(randAngle)) * distAwayHideCheck;
            //maybe also check if it is possible to go there with navmesh.
            if (Physics.Raycast(possibleHidingSpots[i], threatPos - possibleHidingSpots[i], out RaycastHit hit) && (chosenCharacter == null || hit.collider.transform.parent != chosenCharacter.transform))
            {
                chosenHidingSpot = i;
                return possibleHidingSpots[i];
            }
        }
        return transform.position;//maybe instead store the position of the place where they last hid. #todo
    }
    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < possibleHidingSpots.Length; i++)
        {
            Gizmos.color = Color.red;
            if (i == chosenHidingSpot)
                Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(possibleHidingSpots[i], 0.5f);
            if (chosenCharacter)
                Gizmos.DrawLine(possibleHidingSpots[i], chosenCharacter.transform.position);
        }
    }

    private bool ShootIntervalPassed() => timeSinceLastShot >= 60 / shotsPerMinute;
    private bool HasBullets() => bulletsLoaded > 0;
    List<TeamedCharacter> possibleThreats;
    List<TeamedCharacter> allies;
    List<TeamedCharacter> threatsInVision;

    private void IdentifySoldiers()
    {
        possibleThreats.Clear();
        allies.Clear();
        threatsInVision.Clear();

        //identify enemies and allies (including possibly player)
        foreach (TeamedCharacter soldier in BattleManager.instance.teamedCharactersInScene)
        {
            if (soldier == this)
                continue;

            if (SameTeamAs(soldier.teamInt))
                allies.Add(soldier);
            else
                possibleThreats.Add(soldier);
        }
        //identify which threats you can see
        for (int i = 0; i < possibleThreats.Count; i++)
        {
            Vector3 directionToThreat = possibleThreats[i].transform.position - transform.position;
            float angleToThreat = Vector3.Angle(transform.forward, directionToThreat);
            bool threatIsInVisionCone = angleToThreat <= fieldOfViewAngle;
            if (!threatIsInVisionCone)
                continue;
            ////Debug.Log(possibleThreats[i].name + " in cone");
            bool lineToThreatUninterrupted = Physics.Raycast(transform.position, directionToThreat, out RaycastHit visionHit, viewDistance);
            if (!lineToThreatUninterrupted)
                continue;
            bool lineCollisionIsTeamedCharacter = visionHit.collider.transform.parent != null && (visionHit.collider.transform.parent == possibleThreats[i].transform);
            bool playerIsInSight = lineToThreatUninterrupted && lineCollisionIsTeamedCharacter;
            if (playerIsInSight)
                threatsInVision.Add(possibleThreats[i]);
        }
    }
    private void Reload()
    {
        reloading = true;
        animator.SetTrigger("Reload");
    }
    public void ReplaceMagazineAnimationEvent()
    {
        bulletsLoaded = magazineSize;
        reloading = false;
    }


    public override void Die()
    {
        base.Die();

        if (isCommander)
        {
            foreach (TeamedCharacter soldier in BattleManager.instance.teamedCharactersInScene)
            {
                if (soldier == this)
                    continue;

                if (SameTeamAs(soldier.teamInt) && soldier is GenericSoldierDudeEnemyScript script)
                    script.SetAsCommanderContextMenu();
            }
        }

        bloodParticles.transform.parent = null;
        bloodParticles.Play();
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 999, groundLayerMask))
            bloodParticleCollisionPlane.position = hit.point;
        else
            bloodParticleCollisionPlane.position = Vector3.down * 9999;

        BattleManager.instance.teamedCharactersInScene.Remove(this);//erm
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (BattleManager.instance.teamedCharactersInScene.Contains(this))
            BattleManager.instance.teamedCharactersInScene.Remove(this);
    }
    [ContextMenu("SetAsCommander")]
    public void SetAsCommanderContextMenu()
    {
        isCommander = true;
        commanderIndicatorMesh.gameObject.SetActive(true);
        //physical turn on
    }
    public override void TakeDamage(Collider hitColider, int damage)
    {
        if (soldierState == SoldierState.CommandOrObide)
        {
            //Debug.Log("SOMEONE IS SHOOTING AT ME! i'm running for cover away from my last position");
            navMeshAgent.SetDestination(FindCoverPosition(transform.position));
            soldierState = SoldierState.RunForCover;//find a spot?
        }
        base.TakeDamage(hitColider, damage);
    }

    public void SetTeamInt(int newTeamInt)
    {
        teamInt = newTeamInt;

        armEmblemWrap.material = TeamManager.instance.teams[teamInt].identificationAccesoryMaterial;
    }
    public bool isSoldierCommander()
    {
        return isCommander;
    }
}

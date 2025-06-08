using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenericSoldierDudeEnemyScript : Enemy
{
    [Header("State Management")]
    [SerializeField] private SoldierState soldierState;
    private enum SoldierState
    {
        Idle,//only spotting(temporary. replace with somethign about team stuff)
        ShootInSpot,//chance to be ShootInSpot or ShootWhileRunning
        ShootWhileRunning,
        RunForCover,//when arrived.go to reload
        Reloading,
        SeekingOut,//chance of seeking out the guy where they last seen him or holding the angle.
        Holding// random time of deciding to seek out.
    }
    [Header("Comanding")]
    private bool isCommander;
    [Header("Remembering")]
    private Vector3 rememberedChosenThreatPosition;
    private Vector3 positionAtWhichThreatWasSeenBeforeFleeing;// :D
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
    [Header("Running")]
    [SerializeField] private float speed;
    [Header("Hiding")]
    [SerializeField] private int numberOfRandomAttemptsToFindHidingSpot = 10;
    [SerializeField] private int distAwayHideCheck = 10;
    [Header("Blood Particles")]
    [SerializeField] private ParticleSystem bloodParticles;
    [SerializeField] private Transform bloodParticleCollisionPlane;
    [SerializeField] private LayerMask groundLayerMask;
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
    private void FixedUpdate()
    {
        switch (soldierState)
        {
            case SoldierState.Idle:
                IdentifySoldiers();
                if (threatsInVision.Count > 0)
                {
                    if (threatsInVision.Contains(GameReferenceManager.instance.playerTeamHandling))//player in vision and is enemy.
                    {
                        chosenCharacter = GameReferenceManager.instance.playerTeamHandling;//prioritize fighting player
                    }
                    else
                        chosenCharacter = threatsInVision[0];//if not fight the first soldier in vision

                    if (Random.value > 0.5)//random choice
                    {
                        soldierState = SoldierState.ShootInSpot;
                    }
                    else
                    {
                        soldierState = SoldierState.ShootWhileRunning;
                    }
                }
                break;
            case SoldierState.ShootInSpot:
                if (chosenCharacter == null)
                {
                    soldierState = SoldierState.Idle;//idk. maybe replace with run for cover or smth
                    break;
                }
                //still have vision of threat?
                bool hitInThreatDir = Physics.Raycast(transform.position, chosenCharacter.transform.position - transform.position, out RaycastHit visionHit, viewDistance);
                if (hitInThreatDir == false || visionHit.transform != chosenCharacter.transform)
                {
                    soldierState = SoldierState.SeekingOut;
                    break;
                }
                lookAtTransform = chosenCharacter.transform;
                if (!HasBullets())
                {
                    navMeshAgent.SetDestination(FindCoverPosition(chosenCharacter.transform.position));
                    soldierState = SoldierState.RunForCover;//find a spot?
                    positionAtWhichThreatWasSeenBeforeFleeing = transform.position;//for finding enemy after reload
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
                lookAtTransform = chosenCharacter.transform;
                break;
            case SoldierState.RunForCover:
                //find a spot. or maybe when switching
                //once there. if no bullets. reload.
                //if you see an enemy. have a chance to run for cover again.
                break;
            case SoldierState.Reloading:
                if (!HasBullets() && !reloading)
                {
                    Reload();
                }
                else
                {

                }
                break;
            default:
                break;
        }
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
            if (Physics.Raycast(possibleHidingSpots[i], threatPos - possibleHidingSpots[i], out RaycastHit hit) && hit.transform != chosenCharacter.transform)
            {
                chosenHidingSpot = i;
                return possibleHidingSpots[i];
            }
        }
        return transform.position;
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
            bool lineToThreatUninterrupted = Physics.Raycast(transform.position, directionToThreat, out RaycastHit visionHit, viewDistance);
            bool lineCollisionIsTeamedCharacter = visionHit.collider.transform.parent != null && (visionHit.collider.transform.parent.CompareTag("Player") || visionHit.collider.transform.parent.CompareTag("Enemy"));
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
        //physical turn on
    }
}

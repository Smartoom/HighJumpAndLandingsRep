using System.Collections.Generic;
using UnityEngine;

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
        Reloading
    }
    [Header("Comanding")]
    private bool cammander;
    [Header("Remembering")]
    private Vector3 rememberedChosenThreatPosition;
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
    [Header("Blood Particles")]
    [SerializeField] private ParticleSystem bloodParticles;
    [SerializeField] private Transform bloodParticleCollisionPlane;
    [SerializeField] private LayerMask groundLayerMask;
    [Header("Blood Particles")]
    [SerializeField] private Animator animator;

    private void Start()
    {
        BattleManager.instance.teamedCharactersInScene.Add(this);
        bulletsLoaded = magazineSize;

        possibleThreats = new();
        allies = new();
        threatsInVision = new();
    }
    private bool SameTeamAs(int otherTeamInt) => otherTeamInt == teamInt;

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

    }
    private void FixedUpdate()
    {
        IdentifySoldiers();

        if (threatsInVision.Count > 0)
        {
            TeamedCharacter chosenCharacter;
            if (threatsInVision.Contains(GameReferenceManager.instance.playerTeamHandling))
            {
                chosenCharacter = GameReferenceManager.instance.playerTeamHandling;
            }
            else
                chosenCharacter = threatsInVision[0];

            Vector3 direction = new Vector3(chosenCharacter.transform.position.x, transform.position.y, chosenCharacter.transform.position.z) - transform.position;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), rotateTowardThreatSpeed * Time.deltaTime);
            if (CanShoot())
            {
                GameObject instBullet = Instantiate(bullet, bulletSpawnPosition.position, Quaternion.identity);
                instBullet.transform.LookAt(chosenCharacter.transform);//replace
                timeSinceLastShot = 0;
                bulletsLoaded--;
            }
            else if (bulletsLoaded <= 0)
            {
                Reload();
            }

        }
    }
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

    private bool CanShoot() => timeSinceLastShot >= 60 / shotsPerMinute && bulletsLoaded > 0 && !reloading;

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
}

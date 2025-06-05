using UnityEngine;

public class GenericSoldierDudeEnemyScript : Enemy
{
    [Header("Spotting")]
    [SerializeField] private float fieldOfViewAngle;//sight angle
    [SerializeField] private float viewDistance;//sight angle
    [Header("Attack")]
    [SerializeField] private float shotsPerMinute;// fire rate
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPosition;
    private float timeSinceLastShot;
    [Header("Blood Particles")]
    [SerializeField] private ParticleSystem bloodParticles;
    [SerializeField] private Transform bloodParticleCollisionPlane;
    [SerializeField] private LayerMask groundLayerMask;

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        Vector3 directionToPlayer = GameReferenceManager.instance.player.position - transform.position;
        float angleToplayer = Vector3.Angle(transform.forward, directionToPlayer);
        bool playerIsInVisionCone = angleToplayer <= fieldOfViewAngle;

        if (playerIsInVisionCone)
        {
            bool playerIsInSight = Physics.Raycast(transform.position, directionToPlayer, out RaycastHit visionHit, viewDistance) && visionHit.collider.transform.parent != null && visionHit.collider.transform.parent.CompareTag("Player");
            if (playerIsInSight)
            {
                //go to face him
                //once in distance
                if (timeSinceLastShot >= 60 / shotsPerMinute)
                {
                    GameObject instBullet = Instantiate(bullet, bulletSpawnPosition.position, Quaternion.identity);
                    instBullet.transform.LookAt(GameReferenceManager.instance.player.position);
                    timeSinceLastShot = 0;
                }
            }
        }

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

        Destroy(gameObject);
    }
}

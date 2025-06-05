using System.Collections.Generic;
using UnityEngine;

public class AutomaticRifle : MonoBehaviour
{
    [SerializeField] private float shotsPerMinute;// fire rate
    private float timeSinceLastShot;
    [SerializeField] private int damage = 4;
    [SerializeField] private int bullets = 9999;
    //private int maxBullets;
    [SerializeField] private LayerMask shootableMask;
    [Header("Crosshair")]
    [SerializeField] private float crosshairRotationSpeed;
    [SerializeField] private float rotationPerShot = 90;
    private float startCrosshairRotation = 45;
    private int turnsToComplete = 0;
    [Header("References")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactParticles;
    [SerializeField] private Transform Camera;

    private bool CanShoot() => timeSinceLastShot >= 60 / shotsPerMinute && bullets > 0;

    private void OnEnable()
    {
        if (CanvasReferenceManager.instance == null)
            return;
        CanvasReferenceManager.instance.DeactivateCrosshairs();
        CanvasReferenceManager.instance.xCrosshair.gameObject.SetActive(true);
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        if (Input.GetKey(KeyCode.Mouse0) && CanShoot())
        {
            Shoot();
        }

        if (turnsToComplete > 0)
        {
            Quaternion targetQuaternion = Quaternion.Euler(0, 0, rotationPerShot * turnsToComplete + startCrosshairRotation);
            float angleDiff = Quaternion.Angle(CanvasReferenceManager.instance.xCrosshair.localRotation, targetQuaternion);
            float angleToRotateUsingSpeed = Time.deltaTime * crosshairRotationSpeed;
            if (angleDiff < angleToRotateUsingSpeed)//might think about this later. 
            {
                CanvasReferenceManager.instance.xCrosshair.localRotation = targetQuaternion;
                startCrosshairRotation = CanvasReferenceManager.instance.xCrosshair.localRotation.eulerAngles.z;
                turnsToComplete = 0;
            }
            else//rotate
                CanvasReferenceManager.instance.xCrosshair.localRotation *= Quaternion.AngleAxis(Mathf.Min(angleToRotateUsingSpeed, angleDiff), Vector3.forward);
        }

        //debug
        timeSinceDeletingPos += Time.deltaTime;
        if (timeSinceDeletingPos >= debugPosLifeTime && debugShotPosition.Count > 0)
        {
            debugShotPosition.RemoveAt(0);
            timeSinceDeletingPos = 0;
        }
    }
    private readonly List<Vector3> debugShotPosition = new();
    float timeSinceDeletingPos;
    readonly float debugPosLifeTime = 0.5f;
    private void Shoot()
    {
        turnsToComplete++;
        timeSinceLastShot = 0;
        bullets--;
        muzzleFlash.Play();

        if (Physics.Raycast(Camera.position, Camera.forward, out RaycastHit hit, 999, shootableMask))
        {
            Instantiate(impactParticles, hit.point, Quaternion.LookRotation(hit.normal));

            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(hit.collider, damage);
        }
        debugShotPosition.Add(hit.point);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(Camera.position, Camera.position + Camera.forward);

        Gizmos.color = Color.red;
        foreach (Vector3 pos in debugShotPosition)
        {
            Gizmos.DrawWireSphere(pos, 0.25f);
        }
    }
}

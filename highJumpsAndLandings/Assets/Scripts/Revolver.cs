using System.Collections.Generic;
using UnityEngine;

public class Revolver : MonoBehaviour
{
    [SerializeField] private float shotsPerMinute;// fire rate
    [Range(0, 1.5f)]
    /*[SerializeField] */
    private float timeSinceLastShot = 10;
    [SerializeField] private int damage = 4;
    [SerializeField] private int bullets = 9999;
    //private int maxBullets;
    [SerializeField] private LayerMask shootableMask;
    /*    [Header("Crosshair")]
        [SerializeField] private float normalOffset;
        [SerializeField] private float increasedOffset;
        [SerializeField] private float decreasedOffset;
        [SerializeField] AnimationCurve crosshairOffsetCurve;*/
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
        CanvasReferenceManager.instance.plusCrosshair.gameObject.SetActive(true);
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Mouse0) && CanShoot())
        {
            Shoot();
        }

        /*        // Crosshair
                float crosshairOffset = crosshairOffsetCurve.Evaluate(timeSinceLastShot);
                for (int i = 0; i < CanvasReferenceManager.instance.plusCrosshairBars.Length; i++)
                {
                    CanvasReferenceManager.instance.plusCrosshairBars[i].localPosition = Vector3.right * crosshairOffset;
                }*/

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
        timeSinceLastShot = 0;
        bullets--;
        if (muzzleFlash)
            muzzleFlash.Play();

        if (Physics.Raycast(Camera.position, Camera.forward, out RaycastHit hit, 999, shootableMask))
        {
            if (impactParticles)
                Instantiate(impactParticles, hit.point, Quaternion.LookRotation(hit.normal));

            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(hit.collider, damage, hit.point);
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
    /*    public AnimationCurve GetCrosshairOffsetCurve()
        {
            return crosshairOffsetCurve;
        }*/
    public float GetTimeSinceLastShot()
    {
        return timeSinceLastShot;
    }
    /// <summary>
    /// Only used by editor
    /// </summary>
    /// <param name="value"></param>
    public void SetTimeSinceLastShot(float value)
    {
        timeSinceLastShot = value;
    }
}

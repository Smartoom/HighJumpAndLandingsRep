using UnityEngine;

public class PlayerBow : MonoBehaviour
{
    [SerializeField] private int arrows = 9999;
    [Header("Pulling")]
    [SerializeField] private float timeToReadyForShot;//doesn't have to compelte the animation. 
    private float timeReadied;
    private bool wasReadyingLastFrame;
    [Header("Crosshair")]
    [SerializeField] private float readiedCrosshairSize;
    //private int maxArrows;
    [Header("References")]
    [SerializeField] private Transform Camera;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnTransform;//position and direction

    private void OnEnable()
    {
        if (CanvasReferenceManager.instance == null)
            return;
        CanvasReferenceManager.instance.DeactivateCrosshairs();
        CanvasReferenceManager.instance.circleCrosshair.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            timeReadied += Time.deltaTime;
            wasReadyingLastFrame = true;
        }
        else
        {
            if (wasReadyingLastFrame && timeReadied >= timeToReadyForShot && arrows > 0)
            {
                ShootArrow();
            }
            timeReadied = 0;
            wasReadyingLastFrame = false;
        }
        CanvasReferenceManager.instance.circleCrosshair.localScale = Vector3.one * Mathf.Lerp(1, readiedCrosshairSize, timeReadied / timeToReadyForShot);
    }
    private void ShootArrow()
    {
        Instantiate(arrowPrefab, arrowSpawnTransform.position, arrowSpawnTransform.rotation);
        arrows--;
    }
}

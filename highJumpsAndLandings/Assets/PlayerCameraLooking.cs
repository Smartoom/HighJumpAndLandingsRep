using UnityEngine;

public class PlayerCameraLooking : MonoBehaviour
{
    [SerializeField] private float dist;
    [SerializeField] private float downMag;
    [SerializeField] private Transform cam;
    [SerializeField] private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 targetPos = cam.position + cam.forward*dist + Vector3.down * Mathf.Min(downMag * collision.impulse.magnitude,dist);
    }
}

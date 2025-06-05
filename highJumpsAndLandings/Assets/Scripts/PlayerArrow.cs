using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float distanceOfFeatherTipFromCollision;
    private Vector3 lastPosition;
    private void Update()
    {
        lastPosition = transform.position;
        transform.position += speed * Time.deltaTime * transform.forward;
        if (Physics.Raycast(lastPosition, transform.forward, out RaycastHit hit, speed * Time.deltaTime))//hit something
        {
            transform.SetParent(hit.collider.transform, true);
            transform.position = hit.point + -transform.forward * distanceOfFeatherTipFromCollision;
            enabled = false;

            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(hit.collider, damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(lastPosition, lastPosition + transform.forward * speed);
    }
}

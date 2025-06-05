using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float radius;
    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private float bulletDeathDistance;//might switch to just timed death instead.

    private void FixedUpdate()
    {
        transform.position += speed * Time.fixedDeltaTime * transform.forward;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        if (colliders.Length > 0)
        {
            if (colliders[0].transform.parent != null && colliders[0].transform.parent.CompareTag("Player"))//might cause a bug. since only the first contact is used. too lazy to do anythign
            {
                colliders[0].transform.GetComponentInParent<PlayerHealth>().TakeDamage(damage);
            }
            collisionParticles.transform.parent = null;
            collisionParticles.Play();
            Destroy(gameObject);
            return;
        }
        if (transform.position.sqrMagnitude > bulletDeathDistance * bulletDeathDistance)
            Destroy(gameObject);
    }
}

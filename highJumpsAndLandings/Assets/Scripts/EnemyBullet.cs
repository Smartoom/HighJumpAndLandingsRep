using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private float bulletDeathTime;//might switch to just timed death instead.
    //[SerializeField] private Rigidbody rb;
    private void Start()
    {
        Destroy(gameObject, bulletDeathTime);
    }
    Vector3 lastPos;
    private void FixedUpdate()
    {
        float distance = speed * Time.fixedDeltaTime;
        transform.position += distance * transform.forward;
        bool hitSomething = Physics.Raycast(lastPos, transform.forward, out RaycastHit hit, distance);
        if (hitSomething)
        {
            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
            else if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<Enemy>().TakeDamage(hit.collider, damage, hit.point);
            }
            collisionParticles.transform.parent = null;
            collisionParticles.Play();
            Destroy(gameObject);
            return;
        }

        lastPos = transform.position;
    }

/*    private void OnCollisionEnter(Collision collision)
    {
        collisionParticles.transform.parent = null;
        collisionParticles.Play();
        if (collision.transform.parent.CompareTag("Player"))
        {
            collision.transform.GetComponentInParent<PlayerHealth>().TakeDamage(damage);
        }
        else if (collision.transform.parent.CompareTag("Enemy"))
        {
            collision.transform.GetComponentInParent<Enemy>().TakeDamage(collision.collider, damage, collision.contacts[0].point);
        }
        Destroy(gameObject);
    }*/
}

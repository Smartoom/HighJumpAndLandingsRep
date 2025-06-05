using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int health = 20;
    [System.Serializable]
    public class VulnerableHitBox
    {
        public Collider headHitBox;
        public float damageMultiplier = 1;
    }
    [SerializeField] private VulnerableHitBox[] vulnerableHitBoxes;

    public int teamInt = -1;

    /// <summary>
    /// hitColider can be passed as null
    /// </summary>
    public virtual void TakeDamage(Collider hitColider, int damage)
    {
        for (int i = 0; i < vulnerableHitBoxes.Length; i++)
        {
            if (hitColider == vulnerableHitBoxes[i].headHitBox)
                health -= (int)(damage * vulnerableHitBoxes[i].damageMultiplier);
            else
                health -= damage;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        OnEnemyDeath?.Invoke(this);
    }

    public delegate void OnDeath(Enemy enemy);
    // the event itself
    public event OnDeath OnEnemyDeath;
}

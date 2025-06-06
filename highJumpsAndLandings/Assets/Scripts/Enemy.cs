using UnityEngine;

public class Enemy : TeamedCharacter
{
    [SerializeField] protected int health = 20;
    [System.Serializable]
    public class VulnerableHitBox
    {
        public Collider headHitBox;
        public float damageMultiplier = 1;
    }
    [SerializeField] private VulnerableHitBox[] vulnerableHitBoxes;

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
    /// <summary>
    /// hit point meant for shot knockback. this method should be overriden by children to use ragdolls n stuff.
    /// </summary>
    public virtual void TakeDamage(Collider hitColider, int damage, Vector3 hitPoint)
    {
        TakeDamage(hitColider, damage);
    }

    public virtual void Die()
    {
        OnEnemyDeath?.Invoke(this);
    }

    public delegate void OnDeath(Enemy enemy);
    // the event itself
    public event OnDeath OnEnemyDeath;
}

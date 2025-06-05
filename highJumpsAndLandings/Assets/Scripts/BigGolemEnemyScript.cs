using UnityEngine;

public class BigGolemEnemyScript : Enemy
{
    public override void Die()
    {
        base.Die();

        Destroy(gameObject);
    }
}

using UnityEngine;

public abstract class Enemy : NPC
{
    /// <summary>
    /// Range to perform attack.
    /// </summary>
    public float attackRange;

    /// <summary>
    /// Determines whether or not to maintain a chase.
    /// </summary>
    /// <returns>bool</returns>
    public abstract bool KeepChasing();

    /// <summary>
    /// Determines whether or not to continue attacking.
    /// </summary>
    /// <returns></returns>
    public abstract bool KeepAttacking();

    /// <summary>
    /// Attack method to implement.
    /// </summary>
    public abstract void Attack();
}
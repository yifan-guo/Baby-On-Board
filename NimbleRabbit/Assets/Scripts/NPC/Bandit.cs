using System;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : Enemy 
{
    /// <summary>
    /// Package that Bandit has stolen.
    /// </summary>
    public Package stolenPackage {get; private set;}

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    protected override void Start()
    {
        base.Awake();

        stolenPackage = null;

        Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>
        {
            {typeof(IdleState), new IdleState(this)},
            {typeof(ChaseState), new ChaseState(this)},
            {typeof(AttackState), new AttackState(this)}
        };

        stateMachine.SetStates(states);
    }

    /// <summary>
    /// Keep chase if we need to steal a package.
    /// </summary>
    /// <returns>bool</returns>
    public override bool KeepChasing()
    {
        if (stolenPackage != null)
        {
            return false;
        }

        return PlayerController.instance.packages.Count > 0;
    }

    /// <summary>
    /// Keep trying to steal if we haven't gotten anything and are in range.
    /// </summary>
    /// <returns></returns>
    public override bool KeepAttacking()
    {
        if (stolenPackage != null)
        {
            return false;
        }

        float dist = Vector3.Distance(
            PlayerController.instance.transform.position,
            transform.position);

        return dist < attackRange;
    }

    /// <summary>
    /// Steal a package.
    /// </summary>
    public override void Attack()
    {
        PlayerController player = PlayerController.instance;
        List<Package> pkgs = player.packages;
        if (pkgs.Count == 0)
        {
            return;
        }

        // Randomly pick any package the player has
        int pkgIdx = UnityEngine.Random.Range(
            0,
            pkgs.Count);

        stolenPackage = pkgs[pkgIdx];
        
        player.DropPackage(
            stolenPackage,
            transform);

        Vector3 force = (player.transform.position - transform.position).normalized;
        force *= nav.velocity.magnitude * 2f;
        player.rb.AddForce(
            force,
            ForceMode.Impulse);
    }
}
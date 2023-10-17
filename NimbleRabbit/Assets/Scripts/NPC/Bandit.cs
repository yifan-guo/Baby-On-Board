using System;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : Enemy 
{
    /// <summary>
    /// Package Collector for packages that Bandit has stolen.
    /// </summary>
    public PackageCollector pc {get; private set;} 

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        pc = GetComponent<PackageCollector>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>
        {
            {typeof(IdleState), new IdleState(this)},
            {typeof(ChaseState), new ChaseState(this)},
            {typeof(AttackState), new AttackState(this)},
            {typeof(ApprehendedState), new ApprehendedState(this)}
        };

        stateMachine.SetStates(states);
    }

    /// <summary>
    /// Keep chase if we need to steal a package.
    /// </summary>
    /// <returns>bool</returns>
    public override bool KeepChasing()
    {
        if (pc.packages.Count > 0)
        {
            return false;
        }

        return PlayerController.instance.pc.packages.Count > 0;
    }

    /// <summary>
    /// Keep trying to steal if we haven't gotten anything and are in range.
    /// </summary>
    /// <returns></returns>
    public override bool KeepAttacking()
    {
        if (pc.packages.Count > 0)
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
        List<Package> pkgs = player.pc.packages;

        if (pkgs.Count == 0)
        {
            return;
        }

        // Randomly pick any package the player has
        int pkgIdx = UnityEngine.Random.Range(
            0,
            pkgs.Count);

        Package stolenPackage = pkgs[pkgIdx];
        
        player.pc.DropPackage(
            stolenPackage,
            this.pc);

        Vector3 force = (player.transform.position - transform.position).normalized;
        force *= nav.velocity.magnitude * 2f;
        player.hp.Hit(
            player.rb,
            force);
    }
}
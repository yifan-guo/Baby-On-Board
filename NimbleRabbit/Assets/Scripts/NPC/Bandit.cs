using System;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : NPC 
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
            {typeof(EngineFailureState), new EngineFailureState(this)}
        };

        stateMachine.SetStates(states);
    }

    /// <summary>
    /// Keep chase if we need to steal a package and see the player.
    /// </summary>
    /// <returns>bool</returns>
    public override bool Chase()
    {
        if (pc.packages.Count > 0 ||
            PlayerController.instance.pc.packages.Count == 0)
        {
            return false;
        }

        return CanSee(
                PlayerController.instance.transform.position,
                fovMin: 0.25f,
                visionRangeMin: 10f,
                visionRangeMax: visionRange,
                lineOfSight: true);
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
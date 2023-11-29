using System;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : NPC 
{
    /// <summary>
    /// Package Collector for packages that Bandit has stolen.
    /// </summary>
    public PackageCollector pc {get; private set;} 

    public override string IdleStatusText => "No bum's got a package...";
    public override string ChaseStatusText => "That driver's got loot! @%#$%";
    public override string AttackStatusText => "Gimme that package!";
    public override string FleeStatusText => "See ya sucker!";
    public override string EngineFailureStatusText => "$%#$% my Engine broke!";
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
            {typeof(FleeState), new FleeState(this)},
            {typeof(EngineFailureState), new EngineFailureState(this)}
        };

        stateMachine.SetStates(states);
        stateMachine.OnStateChanged += SetBanditIndicator;
    }

    /// <summary>
    /// Toggles a BanditIndicator for this bandit.
    /// </summary>
    protected void SetBanditIndicator(BaseState state)
    {
        if (BanditIndicator.ACTIVE_STATES.Contains(state.GetType()) == true)
        {
            BanditIndicator.Track(this);
        }
        else
        {
            BanditIndicator.Untrack(this);
        }
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

        na.PlayHonk();

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

        AuditLogger.instance.ar.numBanditSteals++;
    }
}
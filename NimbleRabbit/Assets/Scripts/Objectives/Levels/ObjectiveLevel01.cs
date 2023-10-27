using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;


public class ObjectiveLevel01 : MonoBehaviour, IObjective
{
    
    private string _name = "Level 1: A Simple Delivery";
    public string Name
    {
        get { return _name; }
    }

    private string _description = "Deliver the package to the customer.";
    public string Description
    {
        get { return _description; }
    }

    private IObjective.Status _status = IObjective.Status.NotStarted;
    public IObjective.Status ObjectiveStatus
    {
        get { return _status; }
        set { _status = value; }
    }

    private float _startTime;
    public float StartTime
    {
        get { return _startTime; }
        set { _startTime = value; }
    }

    private float _endTime;
    public float EndTime
    {
        get { return _endTime; }
        set { _endTime = value; }
    }

    /// <summary>
    ///  This is where we'll support multiple packages. A level is basically just a set of packages,
    ///  which are also objectives.
    /// </summary>

    private List<IObjective> _prereqs = new List<IObjective>();
    
    public List<IObjective> prereqs
    {
        get { return _prereqs; }
    }

    public Transform[] ControlledPrereqs;

    // make the list of interface objects visible in Unity Editor
    public void Start() {
        _prereqs = ControlledPrereqs.SelectMany(t => GetComponents(typeof(Component))).OfType<IObjective>().ToList();
    }
    
    private IObjective.PrereqOperator _prereqCompletionOperator = IObjective.PrereqOperator.AND;
    public IObjective.PrereqOperator prereqCompletionOperator
    {
        get { return _prereqCompletionOperator; }
    }

    private IObjective.PrereqOperator _prereqFailureOperator = IObjective.PrereqOperator.AND;
    public IObjective.PrereqOperator prereqFailureOperator
    {
        get { return _prereqFailureOperator; }
    }

    public event Action OnObjectiveUpdated;

    public bool PrimaryCompletionCondition()
    {
        // defer to prereqs
        return true;
    }

    public bool PrimaryFailureCondition()
    {
        // defer to prereqs
        return false;
    }

    public void RaiseObjectiveUpdated()
    {
        OnObjectiveUpdated?.Invoke();
    }
}
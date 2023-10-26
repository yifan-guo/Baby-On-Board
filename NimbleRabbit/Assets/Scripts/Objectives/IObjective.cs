using System;
using System.Collections.Generic;
using UnityEngine;

public interface IObjective
{
    /// <summary>
    /// Simple name for the objective.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Longer description of the objective. I.e. instructions for how to complete it.
    /// </summary>
    public string Description { get; }


    public enum Status
    {
        NotStarted,
        InProgress,
        Complete,
        Failed
    }

    public Status ObjectiveStatus { get; set; }

    /// <summary>
    /// Time the objective was started.
    /// </summary>
    public float StartTime { get; set; }

    /// <summary>
    /// Time the objective was completed/failed.
    /// </summary>
    public float EndTime { get; set; }

    public float TimeElapsedSinceStart
    {
        get
        {
            if (StartTime == 0)
            {
                return 0f;
            }
            return Time.time - StartTime;
        }
    }

    public float DurationAtComplete
    {
        get
        {
            if (StartTime == 0 || EndTime == 0)
            {
                return 0f;
            }
            return EndTime - StartTime;
        }
    }

    /// <summary>
    /// List of additional objectives that must be completed before this objective can be completed.
    /// </summary>
    public List<IObjective> prereqs { get; }

    /// <summary>
    ///  Enumeration of operators to evaluate prereqs.
    /// </summary>
    public enum PrereqOperator
    {
        AND,
        OR
    }

    /// <summary>
    /// This operator determines whether completion requires all prereqs to be complete or just one.
    /// </summary>
    public PrereqOperator prereqCompletionOperator { get; }

    /// <summary>
    /// This operator determines whether failure requires all prereqs to be failed or just one.
    /// </summary>
    public PrereqOperator prereqFailureOperator { get; }

    event Action OnObjectiveUpdated;

    /// <summary>
    ///  This is the main completion condition that must be implemented by the inheriting class.
    ///  It is validated after all prereqs have been validated.
    ///  To defer completion to prereqs, return true here.
    /// </summary>
    public bool PrimaryCompletionCondition();

    /// <summary>
    /// Implement this method to raise the OnObjectiveUpdated event.
    /// This can't be done in the interface because interfaces can't have state and state is needed to raise the event.
    /// https://stackoverflow.com/questions/58796545/event-inheritance-with-c8-default-interface-implementation-traits
    /// Example implementation: OnObjectiveUpdated?.Invoke();
    /// </summary>
    public void RaiseObjectiveUpdated();


    /// <summary>
    /// Default method to start the objective and begin tracking time.
    /// </summary>
    public void StartObjective()
    {
        StartTime = Time.time;
        ObjectiveStatus = Status.InProgress;
        RaiseObjectiveUpdated();
    }

    /// <summary>
    /// Default implementation of logic to check all prereqs and then the primary completion condition.
    /// </summary>
    public void CheckCompletion()
    {
        if (ObjectiveStatus != Status.InProgress)
        {
            Debug.Log("Objective is not in progress: " + Name);
            return;
        }
        if (prereqCompletionOperator == PrereqOperator.AND)
        {
            foreach (IObjective prereq in prereqs)
            {
                // If any prereq is not complete, then the objective is not complete.
                if (prereq.ObjectiveStatus != Status.Complete)
                {
                    return;
                }
            }
        }
        else if (prereqCompletionOperator == PrereqOperator.OR)
        {
            foreach (IObjective prereq in prereqs)
            {
                // If any prereq is complete, then the prereqs are satisfied.
                if (prereq.ObjectiveStatus == Status.Complete)
                {
                    break;
                }
            }
        }

        if (PrimaryCompletionCondition())
        {
            Complete();
        }
    }

    /// <summary>
    /// Default method to set the objective as complete and notify listeners.
    /// </summary>
    public void Complete()
    {
        Debug.Log("Objective complete: " + Name);
        ObjectiveStatus = Status.Complete;
        EndTime = Time.time;
        RaiseObjectiveUpdated();
    }

    /// <summary>
    ///  Use this method to reset the objective to its initial state.
    /// </summary>
    public void ResetStatus()
    {
        foreach (IObjective prereq in prereqs)
        {
            prereq.ResetStatus();
        }
        ObjectiveStatus = Status.NotStarted;
        StartTime = 0;
        EndTime = 0;
        RaiseObjectiveUpdated();
    }

    /// <summary>
    /// This is the main failure condition that must be implemented by the inheriting class.
    /// It's validated after all prereqs have been validated.
    /// </summary>
    public abstract bool PrimaryFailureCondition();

    /// <summary>
    /// Default method to check if prereqs have failed and then check the primary failure condition.
    /// </summary>
    public void CheckFailure()
    {
        if (ObjectiveStatus != Status.InProgress)
        {
            Debug.Log("Objective is not in progress: " + Name);
            return;
        }
        Debug.Log("Checking failure for " + Name);
        if (prereqs != null && prereqs.Count > 0)
        {
            if (prereqFailureOperator == PrereqOperator.AND)
            {
                foreach (IObjective prereq in prereqs)
                {
                    // If any prereq is not failed, then the objective is not failed.
                    if (prereq.ObjectiveStatus != Status.Failed)
                    {
                        return;
                    }
                }
                // If we checked all the prereqs and all of them failed, then the objective is failed.
                Fail();
                return;
            }
            else if (prereqFailureOperator == PrereqOperator.OR)
            {
                foreach (IObjective prereq in prereqs)
                {
                    // If any prereq is failed, then the objective is failed.
                    if (prereq.ObjectiveStatus == Status.Failed)
                    {
                        Fail();
                        return;
                    }
                }
            }
        }
        if (PrimaryFailureCondition())
        {
            Fail();
        }
    }

    /// <summary>
    /// Default method to set the objective as failed and notify listeners.
    /// </summary>
    public void Fail()
    {
        ObjectiveStatus = Status.Failed;
        EndTime = Time.time;
        Debug.Log("Objective failed: " + Name);
        RaiseObjectiveUpdated();
    }
}

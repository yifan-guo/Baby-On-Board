using System.Collections.Generic;
using UnityEngine;

public class ObjectivesList : MonoBehaviour
{
    /// <summary>
    /// Map of objectives to their entries.
    /// </summary>
    private static Dictionary<IObjective, ObjectiveEntry> entries;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        if (entries == null)
        {
            entries = new Dictionary<IObjective, ObjectiveEntry>();
        }
    }

    /// <summary>
    /// Get color associated with the objective entry.
    /// </summary>
    /// <param name="obj"></param>
    public static Color GetColor(IObjective obj)
    {
        if (entries.ContainsKey(obj) == false)
        {
            return Color.white;
        }

        return entries[obj].color;
    }

    /// <summary>
    /// Clears static resources.
    /// </summary>
    public static void ClearAll()
    {
        entries.Clear();
    }

    /// <summary>
    /// Add an entry for the given objective.
    /// </summary>
    /// <param name="obj"></param>
    public void AddEntry(IObjective obj)
    {
        if (entries.ContainsKey(obj) == true)
        {
            return;
        }

        ObjectiveEntry entry = Instantiate(
            UIManager.instance.entryPrefab,
            transform);

        entry.Link(obj);

        entries.Add(
            obj,
            entry);
    }
}
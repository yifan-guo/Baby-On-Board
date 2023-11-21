using System;
using UnityEngine;

public class TimeLogBox : MonoBehaviour
{
    int row;
    int col;
    private float start;

    private void Start()
    {
        string name = gameObject.name;
        col = name[name.Length - 1] - '0';
        row = name[name.Length - 2] - '0';
        start = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        start = Time.time;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        AuditLogger.instance.RecordHeatMapValue(
            col,
            row,
            Time.time - start);
    }
}
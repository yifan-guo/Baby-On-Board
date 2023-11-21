using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class Destructible : MonoBehaviour
{

    public ParticleSystem particlePrefab;
    public Vector3 particleSpawnOffset;


    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Spawn particle prefab
            Quaternion spawnRotation = Quaternion.Euler(-90f, 0f, 0f);
            Vector3 spawnPosition = gameObject.transform.position + particleSpawnOffset;
            ParticleSystem psPrefab = Instantiate(particlePrefab, spawnPosition, spawnRotation) as ParticleSystem;
            Destroy(psPrefab, psPrefab.main.duration);
            Destroy(gameObject);
            AuditLogger.instance.ar.numObjectsDestroyed++;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Require Collider
// Require AudioSource
public class Destructible : MonoBehaviour
{
    //public GameObject particlePrefab;

    public ParticleSystem particlePrefab;


    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Transform particleTransform = new Transform();
        // Spawn particle prefab
    
        //GameObject particle = Instantiate(particlePrefab, gameObject.transform.position, Quaternion.identity);
        ParticleSystem psPrefab = Instantiate(particlePrefab, gameObject.transform.position, Quaternion.identity) as ParticleSystem;
        Destroy(psPrefab, psPrefab.main.duration);
        Destroy(gameObject);

    }
}

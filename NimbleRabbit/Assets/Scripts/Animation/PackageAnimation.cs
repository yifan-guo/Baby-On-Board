using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageAnimation : MonoBehaviour
{
    private GameObject PackageSmoke;
    private GameObject PackageDestroy;

    private float currentHP;

    // Start is called before the first frame update
    void Start()
    {
        //read this package's health value
        currentHP = GetComponent<Package>().hp.currentHealth;

        //animation prefabs
        PackageSmoke = transform.Find("Anime_Smoke").gameObject;
        PackageSmoke.SetActive(false);
        PackageDestroy = transform.Find("Anime_Destroy").gameObject;
        PackageDestroy.SetActive(false);

    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        //read this package's health value
        currentHP = GetComponent<Package>().hp.currentHealth;
        //if package health 51-100, do nothing

        //if package health 1-50, play smoke animation
        if(currentHP > 0 && currentHP <= 50)
        {
            PackageSmoke.SetActive(true);
        }

        //if package health 0, play explosion animation, gameover
        if (currentHP == 0)
        {
            PackageSmoke.SetActive(false);
            PackageDestroy.SetActive(true);
        }

    }
}

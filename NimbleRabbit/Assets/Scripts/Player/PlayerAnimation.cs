using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private GameObject animationPlayerDeath;
    // Start is called before the first frame update
    void Start()
    {
        PlayerController.instance.hp.OnHealthChange += PlayerDeath;
        animationPlayerDeath = transform.Find("AnimationPlayerDeath").gameObject;
    }

    public void PlayerDeath()
    {
        //if current health is 0, activate the explosion prefab so it will play explosion animation
        if (PlayerController.instance.hp.currentHealth == 0)
        {
            animationPlayerDeath.SetActive(true);
        }

    }
}

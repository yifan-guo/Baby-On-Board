using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditIndicator : MonoBehaviour
{
    public GameObject Player;
    public GameObject Bandit;

    public Bandit bandit;

    public Vector3 PlayerPosition;
    public Vector3 BanditPosition;

    public Vector3 distVector;

    public float distance;
    public float angle;

    public Vector3 indicatorOffset; 

    public Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("NewPlayer");
        Bandit = GameObject.Find("Bandit");
        rend = GetComponent<Renderer>();
        rend.enabled = false;
        
        bandit = Bandit.GetComponent<Bandit>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //find positions of player car and bandit
        PlayerPosition = Player.transform.position;
        BanditPosition = Bandit.transform.position;

        //this is the vector from bandit to player car
        distVector = BanditPosition-PlayerPosition;

        //calculate the distance from bandit to player car
        distance = distVector.magnitude;

        if (bandit.stateMachine.currentState.GetType()!= typeof(IdleState))
        {
            rend.enabled = true;
            //find angle between the bandit and forward direction to determine the indicator's rotation
            angle = Vector3.SignedAngle(distVector, Vector3.forward, Vector3.up);

            //make the indicator rotate and point to the bandit
            transform.rotation = Quaternion.Euler(90f, 0f, angle);

            //calculate indicator offset from player car
            indicatorOffset = new Vector3(5f * distVector.normalized.x, 0.8f, 5f * distVector.normalized.z);

            //update indicator's position
            transform.position = PlayerPosition + indicatorOffset;
        }
        else
        {
            rend.enabled = false;
        }
        


    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightManager : MonoBehaviour {

	public CopLight[] LightList;
	public float rotationsPerMinute = 50.0f;
	public float rotationsPerMinute2 = 70.0f;
	private float downTime, upTime, pressTime = 0;
	private float countDown = 0.2f;
	private bool horn = false;
	private float mode = 0;
	private int sirenMode = 0;
	private Light[] lights;
	public GameObject lightParent;
	private AudioSource Sound;
	public AudioClip Siren1;
	public AudioClip Siren2;
	public AudioClip Horn;


	void Awake()
	{
		Sound = GetComponent<AudioSource> ();


	}

	// Use this for initialization
	void Start () {
		LightList = GetComponentsInChildren<CopLight>();
		lights = lightParent.GetComponentsInChildren<Light>();
		foreach (Light light in lights)
		{
			light.enabled = false; //turn off the light at start of game
		}
	}

	public void TurnOnLights() {
		sirenMode = 1;
	}

	public void TurnOffLights() {
		sirenMode = 0;
	}

	// Update is called once per frame
	void Update () {

		for(int i = 0; i < LightList.Length; i++)
		{
			if(LightList[i].rotation == true && sirenMode == 1)
			{
				LightList[i].transform.Rotate(0, 6.0f * rotationsPerMinute * Time.deltaTime, 0);
			}
			else if(LightList[i].rotation == true && sirenMode == 2)
			{
				LightList[i].transform.Rotate(0, 6.0f * rotationsPerMinute2 * Time.deltaTime, 0);
			}
			else if(LightList[i].flickering == true && sirenMode == 1)
			{
				LightList[i].flicker = false;
			}
			else if(LightList[i].flickering == true && sirenMode == 2)
			{
				LightList[i].flicker = true;
			}

		}

		if(sirenMode == 0)
		{
			horn = false;
			mode = 3;
			//No lights and sirens
			foreach (Light light in lights)
			{
				light.enabled = false; //turn off the light at start of game
			}
		}

		else if(sirenMode == 1)
		{
			horn = false;
			mode = 3;
			//Lights only
			foreach (Light light in lights)
			{
				light.enabled = true; //turn on the lights
			}
		}
	}
}

using UnityEngine;
using System.Collections;
[RequireComponent (typeof (CharacterController))]
public class FPSWalker : MonoBehaviour 
{
	public float speed = 6.0f;
	private Vector3 moveDirection = Vector3.zero;
	
	private CharacterController cc;
	
	void Awake()
	{
		cc = GetComponent<CharacterController>();
	}
	

	void FixedUpdate() 
	{
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= speed;
	
		// Move the controller
		cc.Move(moveDirection * Time.deltaTime);
	}


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

	//Vars
	public BoxCollider collBox;
	public Rigidbody body;
	public bool isGrounded = false;
	private Vector2 forward;


	void Start () {
		forward = transform.forward;
	}
	
	// Update is called once per frame
	void Update () {

	}


	//isGrounded Check
	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.layer == 8 && !isGrounded) {
			isGrounded = true;
		}
	}

	void OnCollisionExit(Collision collision){
		if (collision.gameObject.layer == 8 && isGrounded) {
			isGrounded = false;
		}

	}


	//player Movement and Actions
	public void Move(Vector2 direction){
		//moves with constant speed 
		if (isGrounded) {
			body.AddForce (direction * PlayerGV.G_PlayerRunForce * Time.deltaTime, ForceMode.Impulse);
		}
	}

	public void Move(){
		forward = transform.forward;
		Move (forward);
	}

	public void Jump(){
		//jumps a certain high always
		if (isGrounded) {
			body.AddForce (Vector3.up * PlayerGV.G_PlayerJumpForce, ForceMode.Impulse);
		}
	}

	public void Dig(){


	}

	public void Place(){


	}


	//internal
	private void RotateBodyTo(float input){
		input = Mathf.Clamp (input, 0, 360);
		body.rotation.eulerAngles.Set(0,input,0);
	}
		
}




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

	//Vars
	public BoxCollider collBox;
	public Rigidbody body;
	public bool isGrounded = false;
	private Vector2 forward = Vector2.zero;
	private bool isHolding = false;

	public Vector2 _pos;



	void Start () {
		body.constraints = RigidbodyConstraints.FreezeRotation;
	}
	
	// Update is called once per frame
	void Update () {
		_pos = new Vector2 (transform.position.x, transform.position.z);
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
			RotateBodyTo (direction);
			body.AddForce (new Vector3(direction.x, 0, direction.y) * PlayerGV.G_PlayerRunForce * Time.deltaTime, ForceMode.Impulse);
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
		if (isHolding)
			Drop ();
		else {
			//do dig stuff
		}
	}

	public void Drop(){
		//do opposite of dig stuff
	}

	//internal
	private void RotateBodyTo(Vector2 input){
		transform.rotation = Quaternion.LookRotation(new Vector3(input.x, 0, input.y));
	}

}




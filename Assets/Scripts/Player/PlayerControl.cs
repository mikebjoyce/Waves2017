using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

	public Camera playerCam;

	//Vars
	public Collider collBox;
	public Rigidbody body;
	public bool isGrounded = false;
	private Vector3 forward;
	private bool isHolding = false;
	private bool lockMove = false;

	public Vector2 position;

	public void Initialize(){
		forward = transform.forward;
		transform.position = new Vector3(WorldGrid.worldCenterPoint.x, WorldGrid.Instance.GetHeightAt(WorldGrid.worldCenterPoint) + 5, WorldGrid.worldCenterPoint.y);
	}

	public void Initialize(Vector3 loc){
		transform.position = loc;
	}


	void Start () {
		body.constraints = RigidbodyConstraints.FreezeRotation;
	}
	
	// Update is called once per frame
	void Update () {
		position = new Vector2 (transform.position.x, transform.position.z);
		forward = transform.forward;
	}


	//isGrounded Check

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.layer == 8 && !isGrounded) {
			isGrounded = true;
			lockMove = false;
		}
	} 

	void OnCollision(Collider collision){
		if (collision.gameObject.layer == 8 && !isGrounded) {
			isGrounded = true;
			lockMove = false;
		}
	}

	void OnCollisionExit(Collision collision){
		if (collision.gameObject.layer == 8 && isGrounded) {
			isGrounded = false;
			lockMove = true;
		}
	}


	//player Movement and Actions
	public void Move(Vector3 input){
		Debug.Log ("Input " + input.ToString ());
		//moves with constant speed 
		//Debug.Log("Input " + input.ToString());
		float xAxis = (input.x != 0) ? input.x/Mathf.Abs(input.x): 0;
		//Debug.Log ("yAxis " + xAxis);
		//Debug.Log ("rotate b4 " + transform.rotation);
		transform.Rotate (new Vector3 (0, -xAxis, 0) * PlayerGV.G_PlayerRotateSpeed * Time.deltaTime);
		Debug.Log ("lockMove " + lockMove);

		if(!lockMove)
			body.AddForce (PlayerGV.G_PlayerRunForce * -input.y * forward * Time.deltaTime, ForceMode.Impulse);
		//Debug.Log ("force applied " + PlayerGV.G_PlayerRunForce * -input.y * forward * Time.deltaTime);
		//Debug.Log ("forward " + forward + " Time.DeltaTime" + Time.deltaTime);

		//body.AddForce (moveDir * PlayerGV.G_PlayerRunForce, ForceMode.Impulse);
	}

	/*
	public void Move(){
		Debug.Log ("Move");
		Move (forward);
	}*/

	public void Jump(){
		//jumps a certain high always
		if (isGrounded) {
			body.AddForce (Vector3.up * PlayerGV.G_PlayerJumpForce, ForceMode.Impulse);
			Debug.Log ("Jump");
		}
	}

	public void Dig(){
		Debug.Log ("in Dig");
		if (isHolding)
			Drop ();
		else {
			//do dig stuff
			int standingHigh = (int) WorldGrid.Instance.GetHeightAt (roundV2(position) , true);
			Vector2 trueDir = strongestDir (new Vector2(forward.x,forward.z));
			int diggingHight = (int) WorldGrid.Instance.GetHeightAt (roundV2(position)  + trueDir, true);

			if (standingHigh + 1 <= diggingHight) {
				WorldGrid.Instance.ModGround(roundV2(position) + trueDir, -1);
				isHolding = true;
			} else {
				//too low to dig
			}
		}
	}

	public void Drop(){
		Debug.Log ("in Drop");
		//do opposite of dig stuff
		int standingHigh = (int) WorldGrid.Instance.GetHeightAt (roundV2(position) , true);
		Vector2 trueDir = strongestDir (new Vector2(forward.x,forward.z));
		int diggingHight = (int) WorldGrid.Instance.GetHeightAt (roundV2(position)  + trueDir, true);
		if (standingHigh + 1 >= diggingHight) {
			WorldGrid.Instance.ModGround (roundV2 (position) + trueDir, 1);
			isHolding = false;
			Debug.Log("B4 "+ transform.position);
			transform.position.Set (roundV2 (position).x, transform.position.y, roundV2 (position).y);
			Debug.Log("A4 "+transform.position);
		} else {
			//too high to place
		}
	}

	//internal
	private void RotateBodyTo(Vector3 input){
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(input), Time.deltaTime * PlayerGV.G_PlayerRotateSpeed);
	}
		

	private Vector2 strongestDir(Vector2 input){
		input = input.normalized;
		if (input == Vector2.zero)
			return input;
		if (Mathf.Abs (input.x) > Mathf.Abs (input.y)) {
			return new Vector2 (input.x/Mathf.Abs(input.x), 0).normalized;
		} else {
			return new Vector2 (0, input.y/Mathf.Abs(input.y)).normalized;
		}
	}

	private Vector2 roundV2(Vector2 input){
		float x = (input.x - (int) input.x > 0.5) ? (int) input.x + 1 : (int) input.x;
		float y = (input.y - (int) input.y > 0.5) ? (int) input.y + 1 : (int) input.y;
		return new Vector2(x,y);
	}
}




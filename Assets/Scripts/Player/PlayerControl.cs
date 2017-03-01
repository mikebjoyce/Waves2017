using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

	public Camera playerCam;
    CameraManager camManager;

	//digLocator
	public GameObject digLoc;
	public MeshRenderer digMesh;
    private bool isDigging = false;
    private float dirtStored = 0;
    private float digRateStored = 0;
    private float dropRateStored = 0;
    private float modGroundRate = GV.Pillar_Min_Division * 5; //same amt as water can mod
    
    //Vars
    public Rigidbody body;

	public bool isGrounded = false;
    public Book book;
    public Transform holdingSpot;  //-38 degrees in x

	private Vector3 forward;
	private bool isHolding = false;
	private bool lockMove = false;
    private float air = GameVariable.maxBreath;

    public Vector2 trueDir = new Vector2();

	public GridPos gridPos;

	/*public void Initialize(){
		forward = transform.forward;
		transform.position = new Vector3(WorldGrid.worldCenterPoint.x, WorldGrid.Instance.GetHeightAt(WorldGrid.worldCenterPoint) + 5, WorldGrid.worldCenterPoint.y);
	}*/

	public void Initialize(Vector3 loc){
        camManager = playerCam.transform.parent.GetComponent<CameraManager>();
        transform.position = loc;
        gridPos = GridPos.ToGP(loc);
        visibleDropPillar (true);
		digMesh = digLoc.GetComponent<MeshRenderer> ();
		moveDigIndic ();
	}


	void Start () {
		body.constraints = RigidbodyConstraints.FreezeRotation;
	}
	
	// Update is called once per frame
	void Update () {
        trueDir = strongestDir(new Vector2(forward.x, forward.z));
        digRateStored += Time.deltaTime* modGroundRate;
        dropRateStored += Time.deltaTime * modGroundRate;
        digRateStored = Mathf.Clamp(digRateStored, 0, GV.Pillar_Min_Division);
        dropRateStored = Mathf.Clamp(dropRateStored, 0, GV.Pillar_Min_Division);

        gridPos = GridPos.ToGP(transform); // new Vector2 (transform.position.x, transform.position.z);
		forward = transform.forward;

		moveDigIndic ();

		ChangeDigLocColor ();

		ApplyCurrentForce ();
        
	}


	void OnTriggerEnter(Collider other){
		if (other.gameObject.layer == 8) {
			isGrounded = true;
			lockMove = false;
		}
    } 

	/*
	void OnCollisionStay(Collider collision){
		if (collision.gameObject.layer == 8 && !isGrounded) {
			isGrounded = true;
			lockMove = false;
		}
	} */

	void OnTriggerExit(Collider other){
		if (other.gameObject.layer == 8) {
			isGrounded = false;
			lockMove = true;
		}
    }


	//player Movement and Actions
	public void Move(Vector3 input){  //move to FixedUpdate and use RigidBody.Move for optimal
		//moves with constant speed 
		//Debug.Log("Input " + input.ToString());
		if (!isMaxSpeed()) {
			float xAxis = (input.x != 0) ? input.x / Mathf.Abs (input.x) : 0;
			//Debug.Log ("yAxis " + xAxis);
			//Debug.Log ("rotate b4 " + transform.rotation);
			transform.Rotate (new Vector3 (0, -xAxis, 0) * PlayerGV.G_PlayerRotateSpeed * Time.deltaTime);

			if (!lockMove) {
				body.AddForce (PlayerGV.G_PlayerRunForce * -input.y * forward * Time.deltaTime, ForceMode.Impulse);
			} else {
				body.AddForce (PlayerGV.G_PlayerRunForce * -input.y * forward * Time.deltaTime * PlayerGV.G_NonGroundedMoveModifier, ForceMode.Impulse);
			}
			//Debug.Log ("force applied " + PlayerGV.G_PlayerRunForce * -input.y * forward * Time.deltaTime);
			//Debug.Log ("forward " + forward + " Time.DeltaTime" + Time.deltaTime);

			//body.AddForce (moveDir * PlayerGV.G_PlayerRunForce, ForceMode.Impulse);
		} else {
			//Debug.Log ("Hit Max Speed");
		}
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
		}
	}

    public void BookActionButton()
    {
        if (!book && Vector3.Distance(GV.theOneBook.transform.position, transform.position) < 2)
        {
            book = GV.theOneBook;
            book.isHeld = true;
            book.transform.SetParent(holdingSpot);
            //picks up book
        }
        else if (book)
        { //drop book
            book.SetHeld(false);
            book.transform.SetParent(null);
            book = null;
            //will drop book
        }
    }

    public void DigButton()
    {
       Dig();
    }

	public void Dig(){
        //do dig stuff
        if (dirtStored < GV.Player_Dirt_Store_Max && digRateStored >= GV.Pillar_Min_Division && canDig())
        {
            WorldGrid.Instance.ModPillarHeight(gridPos + trueDir, GV.PillarType.Ground, -GV.Pillar_Min_Division);
            digRateStored = 0;
            dirtStored += GV.Pillar_Min_Division;
            //dirtStored += m
        }
	}

    public void PanCam(float amt)
    {
        camManager.Pan(amt);
    }

	public void Drop()
    {
        if (dirtStored >= GV.Pillar_Min_Division && dropRateStored >= GV.Pillar_Min_Division && canDrop())
        {
            WorldGrid.Instance.ModPillarHeight(gridPos + trueDir, GV.PillarType.Ground, GV.Pillar_Min_Division);
            //WorldGrid.Instance.ModGround (roundV2 (gridPos) + trueDir, GV.Pillar_Min_Division);
            dropRateStored = 0;
            dirtStored -= GV.Pillar_Min_Division;
            //Debug.Log("B4 "+ transform.position);
            //transform.position.Set (roundV2 (position).x, transform.position.y, roundV2 (position).y);
            //Debug.Log("A4 "+transform.position);
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
		int x = (input.x - (int) input.x > 0.5) ? (int) input.x + 1 : (int) input.x;
		int y = (input.y - (int) input.y > 0.5) ? (int) input.y + 1 : (int) input.y;
		return new Vector2(x,y);
	}

	private Vector3 DigLoc(){
		GridPos digPos = gridPos + trueDir;
		int hight = (int) WorldGrid.Instance.GetPillarStaticHeight (digPos, GV.PillarType.Ground);
		return new Vector3 (digPos.x, hight, digPos.y);
	}
		
	private void moveDigIndic(){
        GridPos digPos = gridPos + trueDir;
		digLoc.transform.position = new Vector3 (digPos.x, WorldGrid.Instance.GetPillarStaticHeight(digPos), digPos.y);
	}

	private void visibleDropPillar(bool input){
		digLoc.SetActive (input);
	}
		
	private bool canDig(){
        
		int standingHigh = (int) WorldGrid.Instance.GetPillarStaticHeight(gridPos, GV.PillarType.Ground);
		int diggingHight = (int) WorldGrid.Instance.GetPillarStaticHeight(gridPos  + trueDir, GV.PillarType.Ground);

		return standingHigh + 2 >= diggingHight && standingHigh - 1 <= diggingHight;
	}

	private bool canDrop(){
		int standingHigh = (int)WorldGrid.Instance.GetPillarStaticHeight(gridPos, GV.PillarType.Ground);
        int diggingHight = (int)WorldGrid.Instance.GetPillarStaticHeight(gridPos + trueDir, GV.PillarType.Ground);
        return standingHigh + 1 >= diggingHight;
	}

	private bool isMaxSpeed(){
		return new Vector2 (body.velocity.x, body.velocity.z).magnitude >= PlayerGV.G_MaxSpeed;
	}

	private void ChangeDigLocColor()
    {
		if (canDig ())
			digMesh.material.color = Color.green;
		else
			digMesh.material.color = Color.red;
	}

	private void ApplyCurrentForce(){
        float heightOfWaterUnderUs = (int)WorldGrid.Instance.GetPillarStaticHeight(gridPos, GV.PillarType.Water);
		if (transform.position.y < heightOfWaterUnderUs)
        {
            air -= Time.deltaTime;
			//body.AddForce(underUs.GetCurrent (false) * PlayerGV.G_WaterForcePerCurrent * Time.deltaTime, ForceMode.Impulse);
			//Debug.Log ("Force added to body from current " + underUs.GetCurrent (false) * PlayerGV.G_WaterForcePerCurrent * Time.deltaTime);
		}
        else
        {
            air += Time.deltaTime * GameVariable.breathRegenRate;
        }
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
	public GameVariable.controlerType controlType;
	private string[] keyList = new string[5];
	public PlayerControl pControl;
    bool initialized = false;



	private Vector2 moveDirection;


	public void Initialize(GameVariable.controlerType conTypSet)
    {
        initialized = true;
        controlType = conTypSet;
        pControl = GetComponent<PlayerControl> ();
		setupKeys ();
	}

	private void setupKeys(){
		keyList [0] = controlType.ToString () + "_Horizontal";
		keyList [1] = controlType.ToString () + "_Vertical";
		keyList [2] = controlType.ToString () + "_Jump";
		keyList [3] = controlType.ToString () + "_Dig";
        keyList [4]  = controlType.ToString() + "_PanCam";
    }

	// Use this for initialization
	/*void Start () {
		Initializer ();
	}*/
	
	// Update is called once per frame
	 void Update () {
            moveDirection = new Vector2(Input.GetAxis(keyList[0]), Input.GetAxis(keyList[1]));
		if(!moveDirection.Equals(Vector2.zero))
			pControl.Move (new Vector3(moveDirection.x, moveDirection.y, 0));
		if (Input.GetButtonDown (keyList [2]))
			pControl.Jump ();
		if (Input.GetButtonDown (keyList [3]))
			pControl.ActionButton();
        if (Input.GetAxis(keyList[4]) > .2f || Input.GetAxis(keyList[4]) < -.2f)
            pControl.PanCam(Input.GetAxis(keyList[4]));
        
	}

	public void changeInputType(GameVariable.controlerType toSet){
		controlType = toSet;
		setupKeys ();
	}
}
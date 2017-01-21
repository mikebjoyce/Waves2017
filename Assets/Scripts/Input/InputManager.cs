using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
	private GameVariable.controlerType controlType;
	private string[] keyList = new string[4];
	public PlayerControl pControl;

	private Vector2 moveDirection;

	void Initializer(GameVariable.controlerType conTypSet, PlayerControl pConSet){
		controlType = conTypSet;
		pConSet = pControl;
		keyList [0] = controlType.ToString () + "_Horizontal";
		keyList [1] = controlType.ToString () + "_Vertical";
		keyList [2] = controlType.ToString () + "_Jump";
		keyList [3] = controlType.ToString () + "_Dig";
	}

	// Use this for initialization
	void Start () {
		Initializer (GameVariable.controlerType.Joy1, GetComponent<PlayerControl> ());
	}
	
	// Update is called once per frame
	 void Update () {
		moveDirection = new Vector2 (Input.GetAxis (keyList [0]), Input.GetAxis (keyList [1]));
		if(!moveDirection.Equals(Vector2.zero))
			pControl.Move (moveDirection);
		if (Input.GetKey (keyList [2]))
			pControl.Jump ();
		if (Input.GetKey (keyList [3]))
			pControl.Dig ();

	}
}

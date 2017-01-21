using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
	public PlayerControl player;
	public Quaternion desiredRot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = player.transform.position;
		desiredRot = player.transform.rotation;
		//transform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation, Time.deltaTime * PlayerGV.G_RotateSpeed);
		transform.rotation = player.transform.rotation;
	}
		
}

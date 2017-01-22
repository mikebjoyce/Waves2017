using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
	public PlayerControl player;
    Camera cam;
	//public Quaternion desiredRot;

    float panValue = 0;
    float maxPanValue = 3.5f;

    Vector3 camStartPos = new Vector3(0, 8.56f, -9.8f);
    Vector3 camStartRot = new Vector3(48.5f, 0, 0);

    Vector3 fullZoomPos = new Vector3(-.58f,.8f,-1.07f);
    Vector3 fullZoomRot = new Vector3(0,0,0);


	// Use this for initialization
	void Start () {
        cam = GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 camPos = Vector3.Slerp(camStartPos, fullZoomPos, panValue / maxPanValue);
        Vector3 camRot = Vector3.Slerp(camStartRot, fullZoomRot, panValue / maxPanValue);
        cam.transform.localPosition = camPos;
        cam.transform.localEulerAngles = camRot;
        //desiredRot = player.transform.rotation + zoomOffset;
        //transform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation, Time.deltaTime * PlayerGV.G_RotateSpeed);

        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
	}

    public void Pan(float f)
    {
        panValue += f * Time.deltaTime;
        panValue = Mathf.Clamp(panValue, 0, maxPanValue);
    }
		
}

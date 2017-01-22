using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
	public PlayerControl player;
	//public Quaternion desiredRot;

    float panValue = 0;
    float maxPanValue = 3.5f;
    Vector3 fullZoomPos = new Vector3(-.55f,-2f,10);
    Vector3 fullZoomRot = new Vector3(-24,0,0);


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 posOffset = Vector3.Slerp(new Vector3(), fullZoomPos, panValue / maxPanValue);
        Vector3 zoomOffset = Vector3.Slerp(player.transform.eulerAngles, fullZoomRot, panValue / maxPanValue);
        transform.position = player.transform.position + posOffset;
        //desiredRot = player.transform.rotation + zoomOffset;
        //transform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation, Time.deltaTime * PlayerGV.G_RotateSpeed);
        transform.eulerAngles = player.transform.eulerAngles + zoomOffset;
		//transform.rotation = player.transform.rotation;
	}

    public void Pan(float f)
    {
        panValue += f * Time.deltaTime;
        panValue = Mathf.Clamp(panValue, 0, maxPanValue);
        Debug.Log("pan value: " + panValue);
    }
		
}

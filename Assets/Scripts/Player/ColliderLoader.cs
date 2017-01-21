using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderLoader : MonoBehaviour {
	public int rad = PlayerGV.G_RadiusOfColliLoad;
	public PlayerControl player;
	public Vector2 lastPos;

	public Pillar[,] pillars;

	// Use this for initialization
	void Start () {
		player = GetComponent<PlayerControl>();
		lastPos = player._pos;
		pillars = new Pillar[2*rad+1,2*rad+1];
	}
	
	// Update is called once per frame
	void Update () {
		if (lastPos != player._pos) {
			DeactivatePillarCol ();
			UpdatePillarArr ();
			ActivatePillarCol ();
		}
		lastPos = player._pos;
	}

	public void UpdatePillarArr(){
		Vector2 pos = player._pos;
		for (int x = -rad; x < rad; x++) {
			for (int y = -rad; y < rad; y++) {
				pillars [x + rad,y + rad] = WorldGrid.Instance.GetPillarAt (new Vector2 (x + pos.x, y + pos.y),true);
			}
		}
	}

	public void ActivatePillarCol(){
		foreach (Pillar p in pillars) {
			if(p != null)
				p.pillarCollider.enabled = true;
		}
	}

	public void DeactivatePillarCol(){
		foreach (Pillar p in pillars) {
			if(p != null)
				p.pillarCollider.enabled = false;
		}
	}
}

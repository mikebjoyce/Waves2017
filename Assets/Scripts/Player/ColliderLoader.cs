using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderLoader : MonoBehaviour {
	public int rad;
	public Vector2 center;
	public PlayerControl player;

	public Pillar[,] pillars;

	void Initialize(int _rad, PlayerControl _player){
		rad = _rad;
		player = _player;
		pillars = new Pillar[2*rad+1,2*rad+1];
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		DeactivatePillarCol ();
		UpdatePillarArr ();
		ActivatePillarCol ();
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
			p.pillarCollider.enabled = true;
		}
	}

	public void DeactivatePillarCol(){
		foreach (Pillar p in pillars) {
			p.pillarCollider.enabled = false;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderLoader : MonoBehaviour {
	public int rad = PlayerGV.G_RadiusOfColliLoad;
	public Vector2 lastPos;

	public Pillar[,] pillars;

	// Use this for initialization
	void Start () {
        //lastPos = MathHelper.V3toV2xz(transform.position);
		pillars = new Pillar[2*rad+1,2*rad+1];
	}
	
	// Update is called once per frame
	void Update () {
		if (lastPos != MathHelper.V3toV2xz(transform.position)) {
			DeactivatePillarCol ();
			UpdatePillarArr ();
			ActivatePillarCol ();
		}
		lastPos = MathHelper.V3toV2xz(transform.position);
	}

	public void UpdatePillarArr(){
		Vector3 pos = transform.position;
		for (int x = -rad; x < rad; x++) {
			for (int y = -rad; y < rad; y++) {
				pillars [x + rad,y + rad] = WorldGrid.Instance.GetPillarAt (new GridPos (x + (int)pos.x, y + (int)pos.z));
			}
		}
	}

	public void ActivatePillarCol(){
		foreach (Pillar p in pillars) {
            if (p != null)
            {
                p.SetColliderActive(true);
            }
		}
	}

	public void DeactivatePillarCol(){
		foreach (Pillar p in pillars) {
			if(p != null)
                p.SetColliderActive(true);
        }
	}
}

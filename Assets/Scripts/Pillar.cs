using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour {

    public GV.PillarType pillarType;
    public Vector3 cord;
    public BoxCollider pillarCollider;
    public Vector2 flowDir;

    public void Initialize(Vector3 _cord, GV.PillarType _pillarType)
    {
        cord = _cord;
        pillarType = _pillarType;
        SetHeight(cord.y);

    }

    public void SetHeight(float newHeight)
    {
        Vector3 newPos = transform.position;
        newPos.y = newHeight;
        transform.position = newPos;
        cord.y = newHeight;
    }

    public float GetHeight()
    {
        return cord.z;
    } 
}

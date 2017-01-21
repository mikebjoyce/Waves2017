using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour {

    public GV.pillarType pillarType;
    public Vector3 cord;


    public float GetHeight()
    {
        return cord.z;
    } 
}

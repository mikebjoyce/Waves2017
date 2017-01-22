using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLinks : MonoBehaviour {

    public Transform groundParent;
    public Transform waterParent;

    public void Awake()
    {
        GV.worldLinks = this;
    }
}

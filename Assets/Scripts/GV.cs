using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GV : MonoBehaviour {

    public enum PillarType { Ground, Water}
    public static readonly Vector3 World_Size = new Vector3(50, 10, 50);
    public static readonly float Water_Sections = 4;
}

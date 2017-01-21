using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GV : MonoBehaviour {

    public enum PillarType { Ground, Water}
    public static readonly Vector2 World_Size = new Vector2(10, 10);
    public static readonly float Water_Sections = 4;
}

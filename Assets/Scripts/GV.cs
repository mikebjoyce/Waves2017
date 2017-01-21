using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GV : MonoBehaviour {

    public enum PillarType { Ground, Water}
    //world
    public static readonly Vector2 World_Size = new Vector2(10, 10);
	public static readonly int World_Size_X = 50;
	public static readonly int World_Size_Y = 50;

    //water
    public static readonly float Water_Sections = 4;
    public static readonly float Water_Flow_Rate = .25f;
    public static readonly List<Vector2> Water_Spread_Directions = new List<Vector2>() { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) }; //randomly selected for even distribution
}

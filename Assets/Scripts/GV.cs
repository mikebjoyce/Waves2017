using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GV : MonoBehaviour {

    public enum PillarType { Ground, Water}

	public static readonly int World_Size_X = 51;
	public static readonly int World_Size_Z = 51;

    //Water
    public static readonly float Water_Update_Time_Step = .5f;
    private static readonly float Water_Sections = 5; //Using 1/this in calculations does not yeild .2f... it just yeilds a nonsensical number that outputs as .2f... use the function below
    public static float Water_Flow;
    public static readonly List<Vector2> Water_Spread_Directions = new List<Vector2>() { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) }; //randomly selected for even distribution
    public static float Water_Sea_Level = 0;


    public static float GetWaterFlowRate()
    {
        return MathHelper.RoundFloat(1 / Water_Sections, 1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GV : MonoBehaviour {

    public enum PillarType { Ground, Water}

    public enum InputType
    {
        KeyboardLeft,
        KeyboardRight,
        Xbox1,
        Xbox2,
        None
    };

    public static readonly int World_Size_X = 41;
	public static readonly int World_Size_Z = 41;

    //Water
    public static readonly float Water_Sea_Width = 3;
    public static readonly float Water_Update_Time_Step = .50f;
    public static readonly float Water_Update_Steps_ = .20f;

    private static readonly float Water_Sections = 5; //Using 1/this in calculations does not yeild .2f... it just yeilds a nonsensical number that outputs as .2f... use the function below
    public static float Water_Flow_Rate; //not readonly, so careful dont overite it
    public static readonly List<Vector2> Valid_Directions = new List<Vector2>() { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) }; //randomly selected for even distribution
    public static readonly float Water_Sea_Level = 0;
    public static readonly float Water_Chance_To_Break_From_Current = .2f;
    public static readonly float Water_Current_Power_Per_Bonus_Mult = .5f;

    //system
    public static float System_Pillar_Cleanup_Interval = 12; //perodically cleans up the strange floaters that appear

    public static void SetupWaterFlowRate()
    {
        Water_Flow_Rate = MathHelper.RoundFloat(1 / Water_Sections, 1);
    }
}

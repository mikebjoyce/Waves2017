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

    public static readonly int World_Size_X = 71;
	public static readonly int World_Size_Z = 71;


    public static WorldLinks worldLinks;
    public static GameFlow gameFlow;
    public static Book theOneBook;

    //Water
    public static readonly float Water_Sea_Width = 3;
    public static readonly float Water_Time_Spent_Updating = 333f; //In milliseconds, max time
    public static readonly float Water_Time_Between_Updates = 0; //In milliseconds 
 

    private static readonly float Water_Sections = 5; //Using 1/this in calculations does not yeild .2f... it just yeilds a nonsensical number that outputs as .2f... use the function below
    public static float Water_Flow_Rate; //not readonly, so careful dont overite it
    public static readonly List<Vector2> Valid_Directions = new List<Vector2>() { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) }; //randomly selected for even distribution
    public static readonly Vector2 Water_Render_Direction = new Vector2(0, 1);
    public static readonly float Water_Sea_Level = 0;
    public static readonly float Water_Chance_To_Break_From_Current = .2f;
    public static readonly float Water_Current_Power_Per_Bonus_Mult = .5f;
    public static readonly float Water_Value_Where_Runs_Renderer_Optz = 20;

    //Tsunami
    public static readonly float Tsunami_Update_Step = .5f;

	//GOD GV
	public static readonly Vector2 GOD_RiseIteration = new Vector2(10,20);
	public static readonly Vector2 GOD_StepsInMax = new Vector2(2,10);
	public static readonly float GOD_flowRatePerPercentAngre = .50f; //range is 0 -> 1
	public static readonly Vector2 GOD_repeats = new Vector2(1,10);


    //Map Generator
    public static int MapGen_Tiles_Load_Per_Cycle = 25;
    public static int MapGen_Tiles_Load_Bonus = 8; //increase by that many per cycle as long as cycle time is under dt below
    public static float MapGen_Ideal_Time_Per_cycle = .50f; //in seconds

    //EarthQuakes
    public static float Earthquake_Tick_Length = 0.5f;

    //system
    public static float System_Pillar_Cleanup_Interval = 40; //perodically cleans up the strange floaters that appear

    public static void SetupWaterFlowRate()
    {
        Water_Flow_Rate = MathHelper.RoundFloat(1 / Water_Sections, 1);
    }
}

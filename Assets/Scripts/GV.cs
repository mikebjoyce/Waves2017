using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GV : MonoBehaviour {

    public enum PillarType { Ground, Water}
    public enum Coord {  North, East, South , West}

    public enum InputType
    {
        KeyboardLeft,
        KeyboardRight,
        Xbox1,
        Xbox2,
        None
    };

    public static readonly int World_Size_X = 16;
	public static readonly int World_Size_Z = 16;


    public static WorldLinks worldLinks;
    public static GameFlow gameFlow;
    public static Book theOneBook;

    //Water
    public static readonly float Water_Sea_Width = 2;
    public static readonly float Water_Time_Spent_Updating = 32f; //In milliseconds, max time
    public static readonly float Water_Time_Between_Updates = .5f; //In milliseconds 
 

    //private static readonly float Water_Sections = 5; //Using 1/this in calculations does not yeild .2f... it just yeilds a nonsensical number that outputs as .2f... use the function below
    public static readonly float Pillar_Min_Division = .2f; //not readonly, so careful dont overite it
    public static readonly List<GridPos> Valid_Directions = new List<GridPos>() { new GridPos(0, 1), new GridPos(0, -1), new GridPos(1, 0), new GridPos(-1, 0) }; //randomly selected for even distribution
    public static readonly Vector2 Water_Render_Direction = new Vector2(0, 1);
    public static readonly float Water_Sea_Level = 0;
    public static readonly float Water_Chance_To_Break_From_Current = .2f;
    public static readonly float Water_Current_Power_Per_Bonus_Mult = .5f;
    public static readonly float Water_Value_Where_Runs_Renderer_Optz = 20;

	//GOD GV
	public static readonly Vector2 GOD_RiseIteration = new Vector2(1,3);
	public static readonly Vector2 GOD_StepsInMax = new Vector2(1,4);
	public static readonly float GOD_flowRatePerPercentAngre = .50f; //range is 0 -> 1
	public static readonly Vector2 GOD_repeats = new Vector2(1,3);

    //Pillar
    public static readonly float Pillar_Height = 15;

    //Map Generator
    public static int MapGen_Tiles_Load_Per_Cycle = 25;
    public static int MapGen_Tiles_Load_Bonus = 8; //increase by that many per cycle as long as cycle time is under dt below
    public static float MapGen_Ideal_Time_Per_cycle = .50f; //in seconds

    //EarthQuakes
    public static float Earthquake_Tick_Length = 0f;

    //Colliders
    public static int colliderUpdateSize = 2;  //2 out in each direction
    //system
    public static float System_Pillar_Cleanup_Interval = 40; //perodically cleans up the strange floaters that appear

    //Players
    public static float Player_Dirt_Store_Max = 20;
    public static float PLAYER_UNSTUCK_TIME = 3;

    public static float RndToMinDiv(float amt)
    {
        int mult = (int)(amt / Pillar_Min_Division);
        return (float)mult * Pillar_Min_Division;
    }
}

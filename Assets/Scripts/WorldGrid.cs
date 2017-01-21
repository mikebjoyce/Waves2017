using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour {

    public Pillar[,] groundGrid = new Pillar[(int)GV.World_Size.x, (int)GV.World_Size.y];
    public Pillar[,] waterGrid = new Pillar[(int)GV.World_Size.x, (int)GV.World_Size.y];

    public float GetHeightAt(Vector2 atLoc)
    {
        return groundGrid[(int)atLoc.x, (int)atLoc.y].cord.z;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPos : ICloneable
{
    public int x, y;

    public GridPos(int _x, int _y)
    {
        x = _x;
        y = _y;
    }


    public static GridPos operator +(GridPos g1, GridPos g2)
    {
        return new GridPos(g1.x + g2.x, g1.y + g2.y);
    }

    public static GridPos operator +(GridPos g1, Vector2 v2)
    {
        return new GridPos(g1.x + (int)v2.x, g1.y + (int)v2.y);
    }

    public static GridPos operator *(GridPos g1, GridPos g2)
    {
        return new GridPos(g1.x * g2.x, g1.y * g2.y);
    }

    public static GridPos operator *(float mult, GridPos g1)
    {
        return new GridPos((int)(g1.x * mult), (int)(g1.y * mult));
    }

    public static GridPos operator *(GridPos g1, float mult)
    {
        return new GridPos((int)(g1.x * mult), (int)(g1.y * mult));
    }

    public override string ToString()
    {
        return string.Format("({0},{1})", x, y);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        GridPos otherGrid = (GridPos)obj;
        return (otherGrid.x == x && otherGrid.y == y);
    }

    public object Clone()  //acts as the  = operator, to pass by clone instead of ref
    {
        return new GridPos(x, y);
    }




    public static GridPos ToGP(Transform t)
    {
        return new GridPos((int)t.position.x, (int)t.position.z);
    }

    public static GridPos ToGP(Vector3 v3)
    {
        return new GridPos((int)v3.x, (int)v3.z);
    }
	
    public static GridPos ToGP(Vector2 v2)
    {
        return new GridPos((int)v2.x, (int)v2.y);
    }

   
}

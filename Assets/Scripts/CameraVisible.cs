using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVisible : MonoBehaviour {

	
    public void UpdateWorld()
    {
        for(int x=0; x < GV.World_Size_X; x++)
        {
            for(int z=0; z < GV.World_Size_Z; z++)
            {
               UpdateLocation(new Vector2(x, z));
            }
        }
    }

    public void UpdatePartial(List<Vector3> toUpdate)
    {
        foreach (Vector3 v3 in toUpdate)
            UpdateLocation(new Vector3(v3.x,v3.z));
    }

    public void UpdateLocation(Vector2 atLoc)
    {
        //Pillar pillarG = WorldGrid.Instance.GetPillarAt(atLoc, true);
        //Pillar pillarP = WorldGrid.Instance.GetPillarAt(atLoc);
        //UpdatePillar(pillarG);
        //if (pillarP && pillarP.pillarType == GV.PillarType.Water)
        //    UpdatePillar(pillarP);
    }


    public void UpdatePillar(Pillar p)
    {
        //Vector2 atLoc = new Vector2(p.pos.x, p.pos.z);
        //Vector2 neighbourLoc = new Vector2();
        //float neighbourHeight;
        //float minHeight = 9999;
        //
        //foreach (Vector2 dir in GV.Valid_Directions)
        //{
        //    neighbourLoc = atLoc + dir;
        //    if (WorldGrid.Instance.GetPillarAt(neighbourLoc, true))
        //    {
        //        neighbourHeight = WorldGrid.Instance.GetHeightAt(neighbourLoc);
        //        if (neighbourHeight < minHeight)
        //            minHeight = neighbourHeight;
        //    }
        //    else
        //        return;
        //}
        //int segBelowMinHeight = p.FindSegmentBelow(minHeight);
        //p.SetInvisibleBelow(segBelowMinHeight);
    }
}

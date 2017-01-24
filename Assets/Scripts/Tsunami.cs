using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tsunami {

    List<ActiveTsunami> activeTsunamis = new List<ActiveTsunami>();

    public void CreateTsunami(ActiveTsunami toCreate, GV.Coord fromCord)
    {
        toCreate.pts = GetLineFromEdge(fromCord);
        activeTsunamis.Add(toCreate);
    }

	public void CreateLineTsunami(Vector2 fromPt, Vector2 toPt, int _stepsToReachMaxPeak, int _stepSpentInMaxPeak, int _ampWaterDivisionsPerStep, bool _repeats, int _repeatSteps)
    {
        //Debug.Log("from: " + fromPt + " to pt: " + toPt + " steptopeak: " + _stepsToReachMaxPeak + " steps in peak: " + _stepSpentInMaxPeak + " divperstep: " + _ampWaterDivisionsPerStep + " rep: " + _repeats + " x " + _repeatSteps);
        _stepsToReachMaxPeak = Mathf.Max(1, _stepsToReachMaxPeak);
        _stepSpentInMaxPeak = Mathf.Max(1, _stepSpentInMaxPeak);
        _ampWaterDivisionsPerStep = Mathf.Max(1, _ampWaterDivisionsPerStep);
        List<Vector2> tsunamiPts = new List<Vector2>();
        if (fromPt.x != toPt.x)
        { //horz tsunami
            if(fromPt.x > toPt.x)
            {
                Vector2 t = fromPt;
                fromPt = toPt;
                toPt = t;
            }
            for (float x = fromPt.x; x <= toPt.x; x++)
                tsunamiPts.Add(new Vector2(x, fromPt.y));
        }
        else if (fromPt.y != toPt.y)
        { //vert tsunami
            if (fromPt.y > toPt.y)
            {
                Vector2 t = fromPt;
                fromPt = toPt;
                toPt = t;
            }
            for (float y = fromPt.y; y <= toPt.y; y++)
                tsunamiPts.Add(new Vector2(fromPt.x,y));
        }

        ActiveTsunami newTsunami = new ActiveTsunami(_stepSpentInMaxPeak, _stepsToReachMaxPeak, _ampWaterDivisionsPerStep, _repeats, _repeatSteps);
        newTsunami.pts = tsunamiPts;
        activeTsunamis.Add(newTsunami);
    }

    private Vector2[] directionToMapEdge(GV.Coord fromCord)
    {
        Vector2[] dir = new Vector2[2];
        switch (fromCord)
        {
            case GV.Coord.North:
                dir[0] = new Vector2(0, GV.World_Size_Z - 1);
                dir[1] = new Vector2(GV.World_Size_X - 1, GV.World_Size_Z - 1);
                break;
            case GV.Coord.South:
                dir[0] = new Vector2(0, 0);
                dir[1] = new Vector2(GV.World_Size_X - 1, 0);
                break;
            case GV.Coord.West:
                dir[0] = new Vector2(0, 0);
                dir[1] = new Vector2(0, GV.World_Size_Z - 1);
                break;
            case GV.Coord.East:
                dir[0] = new Vector2(GV.World_Size_X - 1, 0);
                dir[1] = new Vector2(GV.World_Size_X - 1, GV.World_Size_Z - 1);
                break;
            default:
                dir[0] = new Vector2(0, GV.World_Size_Z - 1);
                dir[1] = new Vector2(GV.World_Size_X - 1, GV.World_Size_Z - 1);
                break;
        }
        return dir;
    }

    private List<Vector2> GetLineFromEdge(GV.Coord edgeCord)
    {
        Vector2[] pts = directionToMapEdge(edgeCord);
        Vector2 fromPt = pts[0];
        Vector2 toPt = pts[1];            

        List<Vector2> tsunamiPts = new List<Vector2>();
        if (fromPt.x != toPt.x)
        { //horz tsunami
            if (fromPt.x > toPt.x)
            {
                Vector2 t = fromPt;
                fromPt = toPt;
                toPt = t;
            }
            for (float x = fromPt.x; x <= toPt.x; x++)
                tsunamiPts.Add(new Vector2(x, fromPt.y));
        }
        else if (fromPt.y != toPt.y)
        { //vert tsunami
            if (fromPt.y > toPt.y)
            {
                Vector2 t = fromPt;
                fromPt = toPt;
                toPt = t;
            }
            for (float y = fromPt.y; y <= toPt.y; y++)
                tsunamiPts.Add(new Vector2(fromPt.x, y));
        }
        return tsunamiPts;
    }
	// Update is called once per frame
	public void UpdateTsunami () {


        List<ActiveTsunami> toDel = new List<ActiveTsunami>();
        foreach (ActiveTsunami at in activeTsunamis)
        {
            if (Time.time > at.timeNextUpdate)
            {
                at.timeNextUpdate = Time.time + GV.Water_Time_Between_Updates;
                if (at.hasPeaked)
                {
                    at.stepsSincePeaked ++;
                    if (at.stepsSincePeaked > at.stepsSpentAtMaxPeak)
                    {
                        if (at.ampStepsApplied <= 0)
                        {
                            if (at.repeats && at.currentRepeatedTimes < at.repeatSteps)
                            {
                                at.currentRepeatedTimes++;
                                ResetTsunami(at);
                            }
                            else
                                toDel.Add(at);
                        }
                        else
                        {
                            foreach (Vector2 pt in at.pts)
                                WorldGrid.Instance.ModGround(pt, -at.ampWaterDivisionsPerStep * GV.Water_Flow_Rate);
                            at.ampStepsApplied--;
                        }
                    }                    
                }
                else
                {
                    if (at.ampStepsApplied >= at.stepsToReachMaxPeak)
                    {
                        at.hasPeaked = true;
                    }
                    else
                    {
                        foreach (Vector2 pt in at.pts)
                            WorldGrid.Instance.ModGround(pt, at.ampWaterDivisionsPerStep * GV.Water_Flow_Rate);
                        at.ampStepsApplied++;
                    }
                }

            }
        }

        foreach (ActiveTsunami del in toDel)
            activeTsunamis.Remove(del);
	}

    private void ResetTsunami(ActiveTsunami toReset)
    {
        toReset.hasPeaked = false;
        toReset.stepsSincePeaked = 0;
        toReset.ampStepsApplied = 0;
        toReset.timeNextUpdate = 0;
        toReset.currentRepeatedTimes = 0;
    }


    public class ActiveTsunami
    {
        public List<Vector2> pts;
        public Vector2 center;
        public bool hasPeaked = false;
        public int stepsSincePeaked = 0;
        public int stepsSpentAtMaxPeak;
        public int stepsToReachMaxPeak;
        public int ampStepsApplied = 0;
        public int ampWaterDivisionsPerStep;
        public float timeNextUpdate = 0;
        public bool repeats;
        public int repeatSteps;
        public int currentRepeatedTimes = 0;

        public ActiveTsunami(int _stepsSpentAtMaxPeak, int _stepsToReachMaxPeak, int _ampWaterDivisionsPerStep, bool _repeats, int _repeatSteps)
        {
            stepsSpentAtMaxPeak = _stepsSpentAtMaxPeak;
            stepsToReachMaxPeak = _stepsToReachMaxPeak;
            ampWaterDivisionsPerStep = _ampWaterDivisionsPerStep;
            repeats = _repeats;
            repeatSteps = _repeatSteps;
        }

    }
}

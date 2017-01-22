using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tsunami {

    List<ActiveTsunami> activeTsunamis = new List<ActiveTsunami>();

	public void CreateLineTsunami(Vector2 fromPt, Vector2 toPt, int _stepsToReachMaxPeak, int _stepSpentInMaxPeak, int _ampWaterDivisionsPerStep, bool _repeats, int _repeatSteps)
    {
        List<Vector2> tsunamiPts = new List<Vector2>();
        if (fromPt.x != toPt.x)
        { //horz tsunami
            if(fromPt.x > toPt.x)
            {
                Vector2 t = fromPt;
                fromPt = toPt;
                toPt = t;
            }
        }
        else if (fromPt.y != toPt.y)
        { //vert tsunami
            if (fromPt.y > toPt.y)
            {
                Vector2 t = fromPt;
                fromPt = toPt;
                toPt = t;
            }            
        }

        for (float x = fromPt.x; x <= toPt.x; x++)
            for (float y = fromPt.y; y <= toPt.y; y++)
            {
                tsunamiPts.Add(new Vector2(x, y));
            }


        
        ActiveTsunami newTsunami = new ActiveTsunami(tsunamiPts, _stepSpentInMaxPeak, _stepsToReachMaxPeak, _ampWaterDivisionsPerStep, _repeats, _repeatSteps);
        activeTsunamis.Add(newTsunami);
    }
	// Update is called once per frame
	public void UpdateTsunami () {


        List<ActiveTsunami> toDel = new List<ActiveTsunami>();
        foreach (ActiveTsunami at in activeTsunamis)
        {
            if (Time.time > at.timeNextUpdate)
            {
                at.timeNextUpdate = Time.time + GV.Tsunami_Update_Step;
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


    private class ActiveTsunami
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

        public ActiveTsunami(List<Vector2> _pts, int _stepsSpentAtMaxPeak, int _stepsToReachMaxPeak, int _ampWaterDivisionsPerStep, bool _repeats, int _repeatSteps)
        {
            pts = new List<Vector2>(_pts);
            stepsSpentAtMaxPeak = _stepsSpentAtMaxPeak;
            stepsToReachMaxPeak = _stepsToReachMaxPeak;
            ampWaterDivisionsPerStep = _ampWaterDivisionsPerStep;
            repeats = _repeats;
            repeatSteps = _repeatSteps;
        }

    }
}

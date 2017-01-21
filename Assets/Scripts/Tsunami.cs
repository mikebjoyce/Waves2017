using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tsunami {

    List<ActiveTsunami> activeTsunamis = new List<ActiveTsunami>();

	public void CreateLineTsunami(Vector2 fromPt, Vector2 toPt, float amp, float _time)
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


        ActiveTsunami newTsunami = new ActiveTsunami();
        newTsunami.pts = new List<Vector2>(tsunamiPts);
        newTsunami.maxTimePeaked = _time;
        newTsunami.ampMax = amp;
        newTsunami.ampGrowRate = 1;
        newTsunami.ampApplied = 0;
        activeTsunamis.Add(newTsunami);
    }
	// Update is called once per frame
	public void UpdateTsunami () {
        List<ActiveTsunami> toDel = new List<ActiveTsunami>();
		foreach(ActiveTsunami at in activeTsunamis)
        {
            if(at.hasPeaked)
            {
                at.timePeaked += Time.deltaTime;
                if (at.timePeaked > at.maxTimePeaked)
                {
                    float ampApplying = Time.deltaTime * -at.ampGrowRate;
                    if (ampApplying + at.ampApplied < 0)
                    {
                        ampApplying = -at.ampApplied;
                        toDel.Add(at);
                    }
                    at.ampApplied += ampApplying;
                    foreach (Vector2 pt in at.pts)
                        WorldGrid.Instance.ModGround(pt, ampApplying);
                }

            }
            else
            {
                float ampApplying = Time.deltaTime * at.ampGrowRate;
                if (ampApplying + at.ampApplied > at.ampMax)
                {
                    at.hasPeaked = true;
                    at.timePeaked = 0;
                    ampApplying = at.ampMax - at.ampApplied;
                }
                at.ampApplied += ampApplying;
                foreach (Vector2 pt in at.pts)
                    WorldGrid.Instance.ModGround(pt, ampApplying);
            }
            
        }

        foreach (ActiveTsunami del in toDel)
            activeTsunamis.Remove(del);
	}


    private class ActiveTsunami
    {
        public List<Vector2> pts;
        public Vector2 center;
        public bool hasPeaked = false;
        public float timePeaked;
        public float maxTimePeaked;
        public float ampMax;
        public float ampApplied = 0;
        public float ampGrowRate;

        public ActiveTsunami()
        {

        }

    }
}

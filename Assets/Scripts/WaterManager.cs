using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterManager  {

    List<Pillar> staticPillars = new List<Pillar>();
    List<Pillar> activeWater = new List<Pillar>();

    public WaterManager()
    {

    }

    public void AddStaticActiveWater(List<Pillar> _staticPillars)
    {
        staticPillars.AddRange(_staticPillars);
        activeWater.AddRange(_staticPillars);
        SortActiveWaterByHeight();
        UpdateWater(activeWater[0]);
    }

    public void UpdateAllWater()
    {
        SortActiveWaterByHeight();
        foreach (Pillar toUpdate in activeWater)
            UpdateWater(toUpdate);
    }

    private void UpdateWater(Pillar toUpdate)
    {
        float actualHeight = toUpdate.GetHeight() - WorldGrid.Instance.groundGrid[(int)toUpdate.pos.x, (int)toUpdate.pos.z].GetHeight();

        //Get random possible spread directions, with the first being the direction of the flow
        Vector2 flowDir = new Vector2(Mathf.RoundToInt(toUpdate.flowDir.x), Mathf.RoundToInt(toUpdate.flowDir.y));
        List<Vector2> spreadDirections = new List<Vector2>(GV.Water_Spread_Directions.OrderBy(item => Random.Range(0, 4)));
        if(spreadDirections.Contains(flowDir))
        {
            spreadDirections.Remove(flowDir);
            spreadDirections.Insert(0, flowDir);
        }
        //Trim the list to only lower heights
        for(int i = spreadDirections.Count - 1; i >= 0; i--)
        {
            float height = WorldGrid.Instance.GetHeightAt(new Vector2(spreadDirections[i].x + toUpdate.pos.x, spreadDirections[i].y + toUpdate.pos.z));
            if (toUpdate.GetHeight() <= height)
            {
                Debug.Log(string.Format("water height {0} vs {1} removed", toUpdate.GetHeight(), height));
                spreadDirections.RemoveAt(i);
            }
        }
        
        string toOut = "spreadDir: ";
        foreach (Vector2 v2 in spreadDirections)
            toOut += v2 + ",";
        Debug.Log(toOut);
    }

    private float CalculateFlow(float flowingHeight, float neighborHeight)
    {
        return 0;
    }

    private void SortActiveWaterByHeight()
    {
        if (activeWater.Count <= 1)
            return;
        
        for (int index1 = 0; index1 < activeWater.Count - 1; index1++)
        {
            for (int index2 = index1 + 1; index2 < activeWater.Count; index2++)
            {
                if(activeWater[index1].GetHeight() < activeWater[index2].GetHeight())
                {
                    Pillar temp = activeWater[index1];
                    activeWater[index1] = activeWater[index2];
                    activeWater[index2] = temp;
                }
            }
        }

        Debug.Log(OutputActiveWaterHeights());
        //active water is now sorted!

    }

    private string OutputActiveWaterHeights()
    {
        string toRet = "Water Heights sorted {";

        foreach (Pillar p in activeWater)
            toRet += p.GetHeight() + ",";
        toRet += "}";
        return toRet;
    }
	
}

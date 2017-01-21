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
    }

    public void UpdateWater()
    {
        SortActiveWaterByHeight();
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

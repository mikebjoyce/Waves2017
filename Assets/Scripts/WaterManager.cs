using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterManager  {

    public List<Pillar> staticPillars = new List<Pillar>();
    List<Pillar> activeWater = new List<Pillar>();
    List<Pillar> toDestroy = new List<Pillar>();

    public WaterManager()
    {

    }

    public void AddStaticActiveWater(List<Pillar> _staticPillars)
    {
        staticPillars.AddRange(_staticPillars);
        activeWater.AddRange(_staticPillars);
        SortActiveWaterByHeight();
        //UpdateWater(activeWater[0]);
    }

    public void UpdateAllWater()
    {
        SortActiveWaterByHeight();
        List<Pillar> toUpdate = new List<Pillar>(activeWater); //Since list will be modified internally
        foreach (Pillar _toUpdate in toUpdate)
            UpdateWater(_toUpdate);
        foreach (Pillar _toDestroy in toDestroy)
            DestroyWater(_toDestroy);
    }

    private void UpdateWater(Pillar toUpdate)
    {
        float actualHeight = toUpdate.GetHeight() - WorldGrid.Instance.groundGrid[(int)toUpdate.pos.x, (int)toUpdate.pos.z].GetHeight();
        //Debug.Log("Updating water at location: " + toUpdate.pos);
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
            Vector2 neighborLoc = new Vector2(spreadDirections[i].x + toUpdate.pos.x, spreadDirections[i].y + toUpdate.pos.z);
            float neighborHeight = WorldGrid.Instance.GetHeightAt(neighborLoc);
            float heightDiff = toUpdate.GetHeight() - neighborHeight;
            if (heightDiff > (1/GV.Water_Sections))
            {
                //Calculate the flow rate
                float groundHeight = WorldGrid.Instance.GetHeightAt(new Vector2(toUpdate.pos.x, toUpdate.pos.z),true);
                float waterDepth = (toUpdate.GetHeight() - groundHeight);
                float percentSurfaceDistributing = 1; //100%
                if (neighborHeight > groundHeight)
                {
                    float neighborRelativeHeight = neighborHeight - groundHeight;
                    percentSurfaceDistributing = 1 -(neighborRelativeHeight / waterDepth);
                }
                float flowRate = waterDepth * percentSurfaceDistributing * GV.Water_Flow_Rate;
               // Debug.Log(string.Format("Flow Rate {0} = waterDepth{1} * percDistr{2} * GV{3}; For pos{4}", flowRate, waterDepth, percentSurfaceDistributing, GV.Water_Flow_Rate, new Vector2(spreadDirections[i].x + toUpdate.pos.x, spreadDirections[i].y + toUpdate.pos.z)));

                //Now spread the water
                Pillar neighborWater = WorldGrid.Instance.waterGrid[(int)neighborLoc.x, (int)neighborLoc.y];
                if (neighborWater != null)
                {//spread water to existing water
                   // Debug.Log(string.Format("adding water at {0} neighborHeight is {1} and added flowrate is {2}", neighborLoc, flowRate, neighborHeight));
                    neighborWater.ModHeight(flowRate);
                    if (toDestroy.Contains(neighborWater))
                        toDestroy.Remove(neighborWater);
                }
                else
                {//do not spread water
                    CreateWater(neighborLoc, flowRate + neighborHeight);
                    //Debug.Log(string.Format("create water at {0} neighborHeight is {1} and added flowrate is {2}",neighborLoc,flowRate,neighborHeight));
                }

                if (!staticPillars.Contains(toUpdate))
                {
                    toUpdate.ModHeight(-flowRate);
                    if (toUpdate.GetHeight() <= 0)
                    {
                        toDestroy.Add(toUpdate);
                        return;
                    }
                }
            }
        }
        
        /*string toOut = "spreadDir: ";
        foreach (Vector2 v2 in spreadDirections)
            toOut += v2 + ",";
        Debug.Log(toOut);*/
    }

    private void DestroyWater(Pillar toDestroy)
    {
        activeWater.Remove(toDestroy);
        WorldGrid.Instance.waterGrid[(int)toDestroy.pos.x, (int)toDestroy.pos.z] = null;
    }

    private void CreateWater(Vector2 loc, float initialHeight)
    {
        GameObject go = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Pillar"), new Vector3((int)loc.x, initialHeight, (int)loc.y), Quaternion.identity) as GameObject;
        Pillar newWater = go.GetComponent<Pillar>();
        newWater.Initialize(new Vector3(loc.x, initialHeight, loc.y), GV.PillarType.Water);
        activeWater.Add(newWater);
        if (WorldGrid.Instance.waterGrid[(int)loc.x, (int)loc.y] != null)
            Debug.LogError("Attempting to create new water to grid where water already existed at loc " + loc);
        WorldGrid.Instance.waterGrid[(int)loc.x, (int)loc.y] = newWater;
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

        //Debug.Log(OutputActiveWaterHeights());
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

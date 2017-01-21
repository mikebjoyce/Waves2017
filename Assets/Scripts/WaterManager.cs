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
        GV.SetupWaterFlowRate();
    }


    public void UpdateAllWater()
    {
        activeWater = SortPillarsByTallestHeight(activeWater);
        List<Pillar> toUpdate = new List<Pillar>(activeWater); //Since list will be modified internally
        foreach (Pillar _toUpdate in toUpdate)
            UpdateWater(_toUpdate);
        foreach (Pillar _toDestroy in toDestroy)
            DestroyWater(_toDestroy);
        toDestroy = new List<Pillar>();
    }

    private bool CanTransfer(float heightDiff, GV.PillarType otherPillarType)
    {
        if (heightDiff <= 0)
            return false;
        return ((heightDiff > GV.Water_Flow_Rate && otherPillarType == GV.PillarType.Ground) || (heightDiff >= GV.Water_Flow_Rate && otherPillarType == GV.PillarType.Water));
    }

    private void UpdateWater(Pillar toUpdate)
    {
        //float actualHeight = toUpdate.GetHeight() - WorldGrid.Instance.groundGrid[(int)toUpdate.pos.x, (int)toUpdate.pos.z].GetHeight();
        //Debug.Log("Updating water at location: " + toUpdate.pos);
        //Get random possible spread directions, with the first being the direction of the flow
        Vector2 flowDir = toUpdate.GetCurrent();
        List<Vector2> spreadDirections = new List<Vector2>(GV.Valid_Directions.OrderBy(item => Random.Range(0, 4)));
        List<Pillar> neighborPillars = new List<Pillar>();
        Pillar flowDirectionPillar = null;
        foreach(Vector2 offset in spreadDirections)
        {
            Vector2 neighborLoc = new Vector2(offset.x + toUpdate.pos.x, offset.y + toUpdate.pos.z);
            float neighborHeight = WorldGrid.Instance.GetHeightAt(neighborLoc);
            float heightDiff = toUpdate.GetHeight() - neighborHeight;
            if (heightDiff > 0)
            {
                heightDiff = MathHelper.RoundFloat(heightDiff, 1);
                Pillar otherPillar = WorldGrid.Instance.GetPillarAt(neighborLoc);
                if (CanTransfer(heightDiff,otherPillar.pillarType))
                //if (heightDiff > GV.GetWaterFlowRate())// (1 / GV.Water_Sections))
                {
                    neighborPillars.Add(otherPillar);
                    if (offset == flowDir && Random.Range(0f,1f) > GV.Water_Chance_To_Break_From_Current)
                        flowDirectionPillar = otherPillar;
                }
            }
        }
        neighborPillars = SortPillarsByShortestHeight(neighborPillars);
        if(toUpdate.DebugLogs)
        {
            string toOut = "neighbors selected in order: ";
            foreach (Pillar v2 in neighborPillars)
            toOut += v2.GetHeight() + ",";
            Debug.Log(toOut);
        }

        if (flowDirectionPillar != null)
        {
            neighborPillars.Remove(flowDirectionPillar);
            neighborPillars.Insert(0, flowDirectionPillar);
        }

        /*if (spreadDirections.Contains(flowDir))
        {
            spreadDirections.Remove(flowDir);
            spreadDirections.Insert(0, flowDir);
        }*/
        

        //Trim the list to only lower heights
        //for(int i = spreadDirections.Count - 1; i >= 0; i--)
        foreach(Pillar neighborPillar in neighborPillars)
        {
            Vector2 neighborLoc = new Vector2(neighborPillar.pos.x, neighborPillar.pos.z);
            float neighborHeight = WorldGrid.Instance.GetHeightAt(neighborLoc);
            float heightDiff = toUpdate.GetHeight() - neighborHeight;
            if (CanTransfer(heightDiff,neighborPillar.pillarType))// > (1/GV.Water_Sections) || neighborPillar.pillarType == GV.PillarType.Water)
            {
                //Calculate the flow rate
                float groundHeight = WorldGrid.Instance.GetHeightAt(new Vector2(toUpdate.pos.x, toUpdate.pos.z),true);
                float waterDepth = (toUpdate.GetHeight() - groundHeight);
                float percentSurfaceDistributing = 1; //100%
                if (neighborHeight > groundHeight)
                {
                    float neighborRelativeHeight = neighborHeight - groundHeight;
                    percentSurfaceDistributing = 1 - (neighborRelativeHeight / waterDepth);
                }
                int rounded = Mathf.RoundToInt(waterDepth * percentSurfaceDistributing);
                rounded = Mathf.Max(1, rounded);
                //float flowRate = waterDepth * percentSurfaceDistributing * GV.Water_Flow_Rate;
                float flowRate = rounded * GV.Water_Flow_Rate;
                //if(toUpdate.DebugLogs) Debug.Log(string.Format("Flow Rate {0} = waterDepth{1} * percDistr{2} * GV{3}; For pos{4}", flowRate, waterDepth, percentSurfaceDistributing, GV.GetWaterFlowRate(), new Vector2(spreadDirections[i].x + toUpdate.pos.x, spreadDirections[i].y + toUpdate.pos.z)));

                //Now spread the water
                if (neighborPillar.pillarType == GV.PillarType.Water && !staticPillars.Contains(neighborPillar))
                {//spread water to existing water
                    if (toUpdate.DebugLogs)
                    {
                        Debug.Log(string.Format("Water at {0} of height {1}, is adding water to {2} at height {3}, the flowrate calculated: {4}", toUpdate.pos, toUpdate.GetHeight(), neighborLoc, neighborHeight, flowRate));
                    }
                    neighborPillar.ModHeight(flowRate);
                    neighborPillar.AddCurrent(new Vector2(toUpdate.pos.x, toUpdate.pos.z), flowRate);
                    if (toDestroy.Contains(neighborPillar))
                        toDestroy.Remove(neighborPillar);
                }
                else if(neighborPillar.pillarType == GV.PillarType.Ground)
                {//do not spread water
                    CreateWater(neighborLoc, flowRate + neighborHeight);
                    if (toUpdate.DebugLogs) Debug.Log(string.Format("create water at {0} neighborHeight is {1} and added flowrate is {2}",neighborLoc,flowRate,neighborHeight));
                }

                if (!staticPillars.Contains(toUpdate))
                {
                    toUpdate.ModHeight(-flowRate);
                    toUpdate.AddCurrent(new Vector2(neighborLoc.x, neighborLoc.y), -flowRate);
                    if (toUpdate.GetHeight() <= groundHeight)
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
        MonoBehaviour.Destroy(toDestroy.gameObject);
    }

    public void CreateWater(Vector2 loc, float initialHeight, bool staticWater = false)
    {
        GameObject go = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Pillar"), new Vector3((int)loc.x, initialHeight, (int)loc.y), Quaternion.identity) as GameObject;
        go.transform.SetParent(GameObject.FindObjectOfType<WorldLinks>().waterParent);
        Pillar newWater = go.GetComponent<Pillar>();
        newWater.Initialize(new Vector3(loc.x, initialHeight, loc.y), GV.PillarType.Water);
        activeWater.Add(newWater);
        if (staticWater)
            staticPillars.Add(newWater);
        WorldGrid.Instance.waterGrid[(int)loc.x, (int)loc.y] = newWater;
    }

    private float CalculateFlow(float flowingHeight, float neighborHeight)
    {
        return 0;
    }

    private List<Pillar> SortPillarsByTallestHeight(List<Pillar> toSort)
    {
        if (toSort.Count <= 1)
            return toSort;
        List<Pillar> toRet = new List<Pillar>(toSort);
        
        for (int index1 = 0; index1 < toRet.Count - 1; index1++)
        {
            for (int index2 = index1 + 1; index2 < toRet.Count; index2++)
            {
                if(toRet[index1].GetHeight() < toRet[index2].GetHeight())
                {
                    Pillar temp = toRet[index1];
                    toRet[index1] = toRet[index2];
                    toRet[index2] = temp;
                }
            }
        }
        return toRet;
    }

    private List<Pillar> SortPillarsByShortestHeight(List<Pillar> toSort)
    {
        if (toSort.Count <= 1)
            return toSort;
        List<Pillar> toRet = new List<Pillar>(toSort);

        for (int index1 = 0; index1 < toRet.Count - 1; index1++)
        {
            for (int index2 = index1 + 1; index2 < toRet.Count; index2++)
            {
                if (toRet[index1].GetHeight() > toRet[index2].GetHeight())
                {
                    Pillar temp = toRet[index1];
                    toRet[index1] = toRet[index2];
                    toRet[index2] = temp;
                }
            }
        }
        return toRet;
    }

    private string OutputActiveWaterHeights()
    {
        string toRet = "Water Heights sorted {";

        foreach (Pillar p in activeWater)
            toRet += p.GetHeight() + ",";
        toRet += "}";
        return toRet;
    }

    public void Debug_DestroyStaticFountain(Pillar toDestroy)
    {
        activeWater.Remove(toDestroy);
        staticPillars.Remove(toDestroy);
        WorldGrid.Instance.waterGrid[(int)toDestroy.pos.x, (int)toDestroy.pos.z] = null;
        MonoBehaviour.Destroy(toDestroy.gameObject);
    }
	
}

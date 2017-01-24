using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterManager  {

    float timeAtNextUpdate = 0;

    public Dictionary<Vector2,Pillar> toUpdate = new Dictionary<Vector2, Pillar>();

    List<Vector2> toAddToUpdate = new List<Vector2>();
    List<Vector2> toRemoveFromUpdate = new List<Vector2>();
    int currentUpdateIndex = 0;

    //sorting algo

    //Vector2 ri = new Vector2();
    int[] renderers;


    public WaterManager()
    {
        GV.SetupWaterFlowRate();
        renderers = new int[GV.World_Size_X];
        for(int i = 0; i < GV.World_Size_X; i++)
            renderers[i] = 0;
    }

    public void WasDisturbed(Pillar disturbedPillar, bool setDisturbed)
    {
        //bool dbg = (disturbedAtLoc == new Vector2(15, 30));
        //if (dbg) Debug.Log("setDisturbed: " + setDisturbed);
        if (setDisturbed)
        {
            //Pillar p = WorldGrid.Instance.GetPillarAt(disturbedAtLoc);
            //if (dbg) Debug.Log("p was found at " + disturbedAtLoc + " : " + (p != null));
              //if (p.DebugLogs) Debug.Log("water type");
              if (!toUpdate.ContainsKey(disturbedPillar.xypos))
              {
                 // if (p.DebugLogs) Debug.Log("added to update manager");
                  toUpdate.Add(disturbedPillar.xypos, disturbedPillar);
              }
        }
        else if (!setDisturbed)
        {
            // if (disturbedAtLoc == new Vector2(15, 30))
            //     Debug.Log("DISTRUBED DELTED");
            toUpdate.Remove(disturbedPillar.xypos);
            //Debug.Log("2");
        }
    }


    public static float safetyExit = 0;
    public static float timeExit = 0;
    public static float loopExit = 0;

    public void UpdateWaterManager()
    {
        if (timeAtNextUpdate < Time.time)
        {
            timeAtNextUpdate = Time.time + GV.Water_Time_Between_Updates;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            bool onePass = true;
            bool canExit = false;
            int initialUpdateIndex = currentUpdateIndex;

            while (!canExit)
            {
                for (int i = currentUpdateIndex; i < GV.World_Size_X; i++)
                {
                    for (int ii = 0; ii < GV.World_Size_Z; ii++)
                    {
                        Vector2 v2 = new Vector2(i, ii);
                        if (toUpdate.ContainsKey(v2))
                        {
                            if (toUpdate[v2].isDisturbed)
                            {
                                UpdateWater(toUpdate[v2]);
                            }
                            else
                            {
                                toUpdate.Remove(v2);
                            }
                        }
                        if (watch.ElapsedMilliseconds > GV.Water_Time_Spent_Updating)
                        {
                            currentUpdateIndex = i;
                            timeExit++;
                            return;
                        }
                    }
                    if (i == initialUpdateIndex)
                    {
                        if (onePass)
                            onePass = false;
                        else
                        {
                            currentUpdateIndex = i;
                            loopExit++;
                            return;
                        }
                    }
                }
                currentUpdateIndex = 0;
            }

        }
        //Debug.Log(watch.Elapsed.Milliseconds);
        //Debug.Log("2: " + watch.Elapsed.Milliseconds);
    }

    /*
    public void UpdateWaterManager()
    {
        // Debug.Log("num in UM: " + toUpdate.Count);
        //remeber your aiming for this one GV.Water_Update_Time_Step = .20f
        Debug.Log("Time: " + Time.time + " -v- " + timeAtNextUpdate + " :Next update");
        if (Time.time > timeAtNextUpdate)
        {
            bool tolerance = true;
            Vector2 startPoint = new Vector2(ri.x, ri.y);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int safety = 0;
            while (true) //If count is very low, instead just use foreach GV.Water_Value_Where_Runs_Renderer_Optz
            {
                for (; ri.x < GV.World_Size_X; ri.x++)
                {
                    for (; ri.y < GV.World_Size_Z; ri.y++)
                    {
                        if (toUpdate.ContainsKey(ri))
                        {
                            if (toUpdate[ri].isDisturbed)
                            {

                                UpdateWater(toUpdate[ri]);
                            }
                            else
                            {
                                toUpdate.Remove(ri);
                            }
                        }

                        System.TimeSpan timeSpan = watch.Elapsed;
                        if (timeSpan.Milliseconds > GV.Water_Time_Spent_Updating)
                        {
                            timeExit++;
                            watch.Stop();
                            timeAtNextUpdate += GV.Water_Time_Between_Updates;
                            return;
                        }

                        safety++;
                        if (safety > 10)
                        {
                            safetyExit++;
                            timeAtNextUpdate += GV.Water_Time_Between_Updates;
                            return;
                        }

                        if(ri == startPoint)
                        {
                            if(!tolerance)
                            {
                                loopExit++;
                                timeAtNextUpdate += GV.Water_Time_Between_Updates;
                                return;
                            }
                            tolerance = false;
                        }
                    }
                }
            }
        }
    }/*
                                watch.Stop();
                                break;

                            
                        }
                    }
                    ri.x = ri.y = 0;
                }
            }
            

            
            // (currentUpdateIndex >= toUpdate.Count)
            //  PrepWaterUpdate();
            
            timeAtNextUpdate = Time.time + GV.Water_Update_Time_Step;           
            var elapsedMs = watch.ElapsedMilliseconds;
        }
       // Debug.Log("num in UM2: " + toUpdate.Count);
    }*/

    private void CheckRenderersSpot(int i)
    {

    }
    

   /* public void UpdateAllWater()
    {
        activeWater = SortPillarsByTallestHeight(activeWater);
        List<Pillar> toUpdate = new List<Pillar>(activeWater); //Since list will be modified internally
        foreach (Pillar _toUpdate in toUpdate)
            UpdateWater(_toUpdate);
        foreach (Pillar _toDestroy in toDestroy)
            DestroyWater(_toDestroy);
        toDestroy = new List<Pillar>();
    }*/

    private bool CanTransfer(float heightDiff, GV.PillarType otherPillarType)
    {
        if (heightDiff <= 0)
            return false;
        return ((heightDiff > GV.Water_Flow_Rate && otherPillarType == GV.PillarType.Ground) || (heightDiff >= GV.Water_Flow_Rate && otherPillarType == GV.PillarType.Water));
    }

    private void UpdateWater(Pillar pillarToUpdate)
    {
        if (!pillarToUpdate.isActive)
        {
            Debug.Log("3");
            toUpdate.Remove(pillarToUpdate.xypos);
            return;
        }
        
        //float actualHeight = toUpdate.GetHeight() - WorldGrid.Instance.groundGrid[(int)toUpdate.pos.x, (int)toUpdate.pos.z].GetHeight();
        //Debug.Log("Updating water at location: " + toUpdate.pos);
        //Get random possible spread directions, with the first being the direction of the flow
        List<Vector2> spreadDirections = new List<Vector2>(GV.Valid_Directions.OrderBy(item => Random.Range(0, 4)));
        List<Pillar> neighborPillars = new List<Pillar>();
        Pillar flowDirectionPillar = null;
        foreach (Vector2 offset in spreadDirections)
        {
            Vector2 neighborLoc = new Vector2(offset.x + pillarToUpdate.pos.x, offset.y + pillarToUpdate.pos.z);
            float neighborHeight = WorldGrid.Instance.GetHeightAt(neighborLoc);
            float heightDiff = pillarToUpdate.GetHeight() - neighborHeight;
            if (heightDiff > 0)
            {
                heightDiff = MathHelper.RoundFloat(heightDiff, 1);
                Pillar otherPillar = WorldGrid.Instance.GetPillarAt(neighborLoc);
                if (CanTransfer(heightDiff,otherPillar.pillarType))
                //if (heightDiff > GV.GetWaterFlowRate())// (1 / GV.Water_Sections))
                {
                    if(pillarToUpdate.GetCurrent(true) == new Vector2() || offset != pillarToUpdate.GetCurrent(true)*-1)
                    {
                        neighborPillars.Add(otherPillar);
                    }
                    
                    if (offset == pillarToUpdate.GetCurrent(true) && Random.Range(0f, 1f) > GV.Water_Chance_To_Break_From_Current)
                    {
                        flowDirectionPillar = otherPillar;
                    }

                }
            }
        }
        if (pillarToUpdate.DebugLogs) Debug.Log("7");
        neighborPillars = SortPillarsByShortestHeight(neighborPillars);
        if(pillarToUpdate.DebugLogs)
        {
            if (pillarToUpdate.DebugLogs) Debug.Log("8");
            string toOut = "neighbors selected in order: ";
            foreach (Pillar v2 in neighborPillars)
            toOut += v2.GetHeight() + ",";
            Debug.Log(toOut);
        }

        if (flowDirectionPillar != null)
        {
            if (pillarToUpdate.DebugLogs) Debug.Log("9");
            neighborPillars.Remove(flowDirectionPillar);
            neighborPillars.Insert(0, flowDirectionPillar);
            /*if(Random.Range(0,.5f) > .5f)
            {
                neighborPillars.Remove(-1 * pillarToUpdate.GetCurrent(true));
            }*/
        }

        if(neighborPillars.Count == 0)
        {
            if (pillarToUpdate.DebugLogs) Debug.Log("10");
            pillarToUpdate.SetCurrent(new Vector2());
            pillarToUpdate.Disturb(false);
            return;
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
            float heightDiff = pillarToUpdate.GetHeight() - neighborHeight;
            if (CanTransfer(heightDiff,neighborPillar.pillarType))// > (1/GV.Water_Sections) || neighborPillar.pillarType == GV.PillarType.Water)
            {
                //Calculate the flow rate
                float groundHeight = WorldGrid.Instance.GetHeightAt(new Vector2(pillarToUpdate.pos.x, pillarToUpdate.pos.z),true);
                float waterDepth = (pillarToUpdate.GetHeight() - groundHeight);
                float percentSurfaceDistributing = 1; //100%
                if (neighborHeight > groundHeight)
                {
                    float neighborRelativeHeight = neighborHeight - groundHeight;
                    percentSurfaceDistributing = 1 - (neighborRelativeHeight / waterDepth);
                }
                int rounded = Mathf.RoundToInt(waterDepth * percentSurfaceDistributing);
                rounded = Mathf.Max(1, rounded);
                //float flowRate = waterDepth * percentSurfaceDistributing * GV.Water_Flow_Rate;
                float flowRate = rounded * GV.Water_Flow_Rate;// * potentialPowerMultiplier;
                /*if(flowDirectionPillar && flowDirectionPillar == neighborPillar)
                { //can apply current bonus
                    Vector2 flowDir = toUpdate.GetCurrent(false);
                    float maxTransfer;
                    if((heightDiff / GV.Water_Flow_Rate) % 2 == 0)
                    {
                        maxTransfer = heightDiff / 2;
                    }
                    else
                    {
                        maxTransfer = heightDiff / 2 + GV.Water_Flow_Rate;
                    }

                    float flowBonus = Mathf.Max(flowDir.x, flowDir.y);
                    flowBonus /= GV.Water_Current_Power_Per_Bonus_Mult;
                    if (flowBonus > 1)
                    {
                        //Debug.Log("flow Rate/bonus/total: " + flowRate + "/" + flowBonus + "/" + (flowRate * flowBonus));
                        flowRate *= flowBonus;
                    }
                    flowRate = Mathf.Min(flowRate, maxTransfer);
                    
                }*/
                //if(toUpdate.DebugLogs) Debug.Log(string.Format("Flow Rate {0} = waterDepth{1} * percDistr{2} * GV{3}; For pos{4}", flowRate, waterDepth, percentSurfaceDistributing, GV.GetWaterFlowRate(), new Vector2(spreadDirections[i].x + toUpdate.pos.x, spreadDirections[i].y + toUpdate.pos.z)));

                //Now spread the water
                if (neighborPillar.pillarType == GV.PillarType.Water && !neighborPillar.isStaticPillar)
                {//spread water to existing water
                    if (pillarToUpdate.DebugLogs)
                    {
                        Debug.Log(string.Format("Water at {0} of height {1}, is adding water to {2} at height {3}, the flowrate calculated: {4}", pillarToUpdate.pos, pillarToUpdate.GetHeight(), neighborLoc, neighborHeight, flowRate));
                    }
                    neighborPillar.ModHeight(flowRate);
                    neighborPillar.AddCurrent(new Vector2(pillarToUpdate.pos.x, pillarToUpdate.pos.z), flowRate);
                   
                }
                else if(neighborPillar.pillarType == GV.PillarType.Ground)
                {//do not spread water
                    Pillar hiddenWaterP = WorldGrid.Instance.waterGrid[(int)neighborLoc.x, (int)neighborLoc.y];
                    if (hiddenWaterP)
                    {
                        hiddenWaterP.SetIsActive(true);
                        hiddenWaterP.Disturb(true);
                        hiddenWaterP.ModHeight(flowRate);
                        hiddenWaterP.AddCurrent(new Vector2(pillarToUpdate.pos.x, pillarToUpdate.pos.z), flowRate);
                    }
                    else
                    {
                        CreateWater(neighborLoc, flowRate + neighborHeight);
                    }
                }

                if (!pillarToUpdate.isStaticPillar)
                {
                    pillarToUpdate.ModHeight(-flowRate);
                    pillarToUpdate.AddCurrent(new Vector2(neighborLoc.x, neighborLoc.y), -flowRate);
                }
            }
        }
    }

    public void CreateWater(Vector2 loc, float initialHeight, bool staticWater = false)
    {
        GameObject go = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Pillar"), new Vector3((int)loc.x, initialHeight, (int)loc.y), Quaternion.identity) as GameObject;
        go.transform.SetParent(GV.worldLinks.waterParent);
        Pillar newWater = go.GetComponent<Pillar>();
        newWater.Initialize(new Vector3(loc.x, initialHeight, loc.y), GV.PillarType.Water);
        //activeWater.Add(newWater);
        if (staticWater)
            newWater.isStaticPillar = true;
        if (!newWater)
            Debug.Log("is null");
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
    
    private void DrainRandomTile()
    {

    }
	
}

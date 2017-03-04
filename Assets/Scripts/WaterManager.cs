using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterManager  {

    float timeAtNextUpdate = 0;

    //public Dictionary<Vector2,Pillar> toUpdate = new Dictionary<Vector2, Pillar>();

    //List<Vector2> toAddToUpdate = new List<Vector2>();
    //List<Vector2> toRemoveFromUpdate = new List<Vector2>();
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
                        GridPos atLoc = new GridPos(i, ii);  //reason doing this is to check each spot in order, so I can timebox cut things off if it takes too long
                        Pillar toUpdate = WorldGrid.Instance.GetPillarAt(atLoc);
                        if (toUpdate.waterActive && toUpdate.isDisturbed)
                            UpdateStaticWater(toUpdate);
                        else
                            Debug.Log(string.Format("water pillar at [{0},{1}] waterActive {2} vs {3} disturbed", atLoc.x, atLoc.y, toUpdate.waterActive, toUpdate.isDisturbed));

                        if (watch.ElapsedMilliseconds > GV.Water_Time_Spent_Updating)
                        {
                            currentUpdateIndex = i;
                            timeExit++;
                            //Debug.Log("Time exit occured, running slow: ");
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
        return ((heightDiff > GV.Pillar_Min_Division && otherPillarType == GV.PillarType.Ground) || (heightDiff >= GV.Pillar_Min_Division && otherPillarType == GV.PillarType.Water));
    }

    private void UpdateStaticWater(Pillar pillarToUpdate)
    {
        Debug.Log("updating as static water");
        if (!pillarToUpdate.waterActive)
        {
            pillarToUpdate.SetDisturbed(false); //okay if ground done first for landslides
            Debug.Log("but was not disturbed");
            return;
        }
        
        float ptu_waterVolume = pillarToUpdate.GetVolume(GV.PillarType.Water);
        Debug.Log("with volume: " + ptu_waterVolume);
        if (ptu_waterVolume <= 0)
        {
            pillarToUpdate.waterActive = false;
            pillarToUpdate.SetDisturbed(false);
            Debug.Log("water trying to update with no volume");
            return;
        }

        float[] amountsToEven = new float[] { 0, 0, 0, 0 };
        int lowestNeighborIndex = 0;
        float lowestNeighborHeight = 999;
        GridPos[] neighborGridLoc = new GridPos[4];
        Pillar[] neighborPillars = new Pillar[4];

        float pillarStaticHeight = pillarToUpdate.GetStaticHeight();
        float lowestNeighborEvenAmt;
        float c = 0;
        float a = 0;


        for (int i = 0; i < 4; i++) //(Vector2 offset in GV.Valid_Directions)
        {
            neighborGridLoc[i] = pillarToUpdate.pos + GV.Valid_Directions[i];
            neighborPillars[i] = WorldGrid.Instance.GetPillarAt(neighborGridLoc[i]);
            float neighborStaticHeight = neighborPillars[i].GetStaticHeight();
            if (neighborStaticHeight >= pillarStaticHeight)
                amountsToEven[i] = 0;
            else
                amountsToEven[i] = (pillarStaticHeight - neighborStaticHeight) / 2;

            if (neighborStaticHeight < lowestNeighborHeight)
            {
                lowestNeighborHeight = neighborStaticHeight;
                lowestNeighborIndex = i;
            }

            c += amountsToEven[i];
        }

        if (lowestNeighborHeight >= pillarStaticHeight)
            return; //Lowest neighbor is taller or even then no transfer
        else
            lowestNeighborEvenAmt = amountsToEven[lowestNeighborIndex];

        c = (c - amountsToEven[lowestNeighborIndex]) / amountsToEven[lowestNeighborIndex];

        if (c >= 0)
        {
            a = (pillarStaticHeight - amountsToEven[lowestNeighborIndex]) / c;
            float totalWouldGive = a * (c + 1);
            if (totalWouldGive > ptu_waterVolume) //if not enough water to give
                a = ptu_waterVolume / (c + 1);

            for (int i = 0; i < 4; i++)
            {
                float amtToGive = (amountsToEven[i] / amountsToEven[lowestNeighborIndex]) * a;
                if (amtToGive > 0)
                    CreateWaterCurrentAt(neighborGridLoc[i], amtToGive, GV.Valid_Directions[i]);
            }

            pillarToUpdate.ModHeight(GV.PillarType.Water, a * (c + 1));
        }
        else  //then formula has a /0
        {
            float amtToGive = a;
            CreateWaterCurrentAt(neighborGridLoc[lowestNeighborIndex], amtToGive, GV.Valid_Directions[lowestNeighborIndex]);
        }
    }

    public void CreateWaterCurrentAt(GridPos pos, float amt, GridPos dir)
    {
        Debug.Log(string.Format("a current was created at pos {0} heading {1} at {2} power", pos, amt, dir));
    }
    /*
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
            //if(Random.Range(0,.5f) > .5f)
            //{
            //    neighborPillars.Remove(-1 * pillarToUpdate.GetCurrent(true));
            //}
        }

        if(neighborPillars.Count == 0)
        {
            if (pillarToUpdate.DebugLogs) Debug.Log("10");
            pillarToUpdate.SetCurrent(new Vector2());
            pillarToUpdate.Disturb(false);
            return;
        }

        //if (spreadDirections.Contains(flowDir))
        //{
        //    spreadDirections.Remove(flowDir);
        //    spreadDirections.Insert(0, flowDir);
        //}

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
                float flowRate = rounded * GV.Pillar_Min_Division;// * potentialPowerMultiplier;
                //if(flowDirectionPillar && flowDirectionPillar == neighborPillar)
                //{ //can apply current bonus
                //    Vector2 flowDir = toUpdate.GetCurrent(false);
                //    float maxTransfer;
                //    if((heightDiff / GV.Water_Flow_Rate) % 2 == 0)
                //    {
                //        maxTransfer = heightDiff / 2;
                //    }
                //    else
                //    {
                //        maxTransfer = heightDiff / 2 + GV.Water_Flow_Rate;
                //    }
                //
                //    float flowBonus = Mathf.Max(flowDir.x, flowDir.y);
                //    flowBonus /= GV.Water_Current_Power_Per_Bonus_Mult;
                //    if (flowBonus > 1)
                //    {
                //        //Debug.Log("flow Rate/bonus/total: " + flowRate + "/" + flowBonus + "/" + (flowRate * flowBonus));
                //        flowRate *= flowBonus;
                //    }
                //    flowRate = Mathf.Min(flowRate, maxTransfer);
                //    
                //}
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
    */

    

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

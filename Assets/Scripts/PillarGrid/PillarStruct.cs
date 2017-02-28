using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarStruct
{
    private Pillar parentPillar;
    private float staticHeight;
    public GV.PillarType pillarType;
    public PillarUI pillarUI;
    float[] current;
    float[] storedCurrent;
    bool isActive = true;

    public PillarStruct(GV.PillarType _pillarType, float _staticHeight, Pillar _parentPillar)
    {
        parentPillar = _parentPillar;
        current = new float[] { 0, 0, 0, 0 };
        storedCurrent = new float[] { 0, 0, 0, 0 };
        if (_pillarType == GV.PillarType.Water)
            isActive = false; //a catch incase, should be actived in setstaticheight
        staticHeight = _staticHeight; //weird order of precedence, hard set here, then ui uses, then SetStaticHeight does visible calculations
        CreatePillarUI(_pillarType);
        SetStaticHeight(_staticHeight);
        //If water, be sure to check ground height to see if to enable or not

    }

    public void SetColliderActive(bool setActive)
    {
        pillarUI.SetColliderActive(setActive);
    }

    private void CreatePillarUI(GV.PillarType ptype)
    {
        //When creating prefab, have prefab default set to false, so they can be turned on if needed. Also weary the collider
        //also theskinning of water
        
        GameObject newPillar = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/Pillar"), new Vector3(parentPillar.pos.x,staticHeight,parentPillar.pos.y), Quaternion.identity);
        pillarUI = newPillar.GetComponent<PillarUI>();
        if (ptype == GV.PillarType.Ground)
        {
            newPillar.transform.SetParent(GV.worldLinks.groundParent);
        }
        else if(ptype == GV.PillarType.Water)
        {
            newPillar.transform.SetParent(GV.worldLinks.waterParent);
            MonoBehaviour.Destroy(newPillar.GetComponent<Collider>());
            float groundHeight = parentPillar.GetStaticHeight(GV.PillarType.Ground);
            if (staticHeight <= groundHeight)
            {
                pillarUI.SetVisible(false);
                staticHeight = groundHeight;
                isActive = false;
            }
            else
            {
                isActive = true;
            }
        }
        pillarUI.Initialize(ptype);
    }

    public float GetStaticHeight()
    {
        return staticHeight;
    }

    public void ModStaticHeight(float modBy)
    {
        SetStaticHeight(staticHeight + modBy);
    }

    public void SetStaticHeight(float newValue)
    {
        staticHeight = newValue;
        
        if (pillarType == GV.PillarType.Water)
        {
            float groundHeight = parentPillar.GetStaticHeight(GV.PillarType.Ground);
            if (staticHeight <= groundHeight)
            {
                isActive = false;
                parentPillar.waterActive = false;
                pillarUI.SetVisible(false);
                staticHeight = groundHeight;
            }
            else if (!isActive && staticHeight > groundHeight)
            {
                isActive = true;
                parentPillar.waterActive = true;
                pillarUI.SetVisible(true);
            }
        }

        pillarUI.HeightModified(staticHeight);
    }

    public float GetVolume()
    {
        Debug.Log("not initialized");
        return 0;
    }

    /*

     public void ModHeight(float modAmt)
    {
        if (DebugLogs) Debug.Log("modHeight called");
        if (isActive)
        {
            if (DebugLogs) Debug.Log("pillar is active");
            Disturb(true);
            if (modAmt < 0)
                DisturbAllNeighbors();
        }
        SetHeight(pos.y + modAmt);
    }

    public void SetHeight(float newHeight)
    {
        Vector3 newPos = transform.position;
        pos.y = newPos.y = newHeight;
        transform.position = newPos;
        
        if (pillarType == GV.PillarType.Water && pos.y <= WorldGrid.Instance.groundGrid[(int)newPos.x, (int)newPos.z].pos.y)
        {
            SetIsActive(false);
            WorldGrid.Instance.waterManager.toUpdate.Remove(xypos);
        }
    }

    public Vector2 GetCurrent(bool normalized)
    {
        if (normalized)
            return MathHelper.CastVectorToOffsetDir(currentDir);

        if (Mathf.Abs(currentDir.x) > .5f && Mathf.Abs(currentDir.x) >= Mathf.Abs(currentDir.y))
            return new Vector2(Mathf.RoundToInt(currentDir.x), 0);
        if (Mathf.Abs(currentDir.y) > .5f && Mathf.Abs(currentDir.y) > Mathf.Abs(currentDir.x))
            return new Vector2(0, Mathf.RoundToInt(currentDir.y));
        return new Vector2();
    }
    
    public void AddCurrent(Vector2 fromLoc, float strength)
    {
        currentDir += new Vector2(pos.x - fromLoc.x, pos.z - fromLoc.y) * strength;
    }

    public void SetCurrent(Vector2 newCur)
    {
        currentDir = newCur;
    }
    
    
    */

}

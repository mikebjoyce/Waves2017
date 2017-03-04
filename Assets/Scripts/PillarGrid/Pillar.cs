using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar {

    //public GV.PillarType pillarType;
    public GridPos pos;
    Dictionary<GV.PillarType, PillarStruct> pillarStructs;
    public bool isActive = true;
   // public bool isStaticPillar = false;
    public bool isDisturbed = false;
    public bool waterActive = false;
    public bool waterInitialized = false;
    //public Vector2 xypos { get { return new Vector2(pos.x, pos.z); }}

    public Pillar(GridPos _pos)//,float _height)
    {
        pos = _pos;
        pillarStructs = new Dictionary<GV.PillarType, PillarStruct>();
    }

    public void InitializePillarStruct(GV.PillarType _ptype, float staticHeight)
    {
        //ground is initialized first, then water
        pillarStructs.Add(_ptype, new PillarStruct(_ptype, staticHeight, this));
        if (_ptype == GV.PillarType.Water)
            waterInitialized = true;
    }

    public PillarStruct GetPillarStruct(GV.PillarType _ptype)
    {
        return pillarStructs[_ptype];
    }

    public void ModHeight(GV.PillarType _ptype, float amt)
    {
        if (_ptype == GV.PillarType.Ground)
        {
            pillarStructs[GV.PillarType.Ground].ModStaticHeight(amt);
            if(waterInitialized)
                pillarStructs[GV.PillarType.Water].ModStaticHeight(amt);
        }
        else if(_ptype == GV.PillarType.Water)
        {
            if(waterInitialized)
                pillarStructs[GV.PillarType.Water].ModStaticHeight(amt);
            else
            {
                InitializePillarStruct(GV.PillarType.Water, pillarStructs[GV.PillarType.Ground].GetStaticHeight() + amt);
                //initialize water
            }
        }
        WorldGrid.Instance.DisturbTargetAndNeighbors(pos);
    }


    public float GetVolume(GV.PillarType _ptype)
    {
        //only works for water atm
        if (!waterActive)
            return 0;
        else
            return pillarStructs[_ptype].GetVolume();
    }

    /*public void SetIsActive(bool _isActive)
    {
        isActive = _isActive;
        gameObject.SetActive(_isActive);
        if (!_isActive)
            isDisturbed = false;
    }*/

    public float GetStaticHeight(GV.PillarType ptype = GV.PillarType.Water) //water will give you tallest height always
    {
        if(!waterActive && ptype == GV.PillarType.Water)
            return pillarStructs[GV.PillarType.Ground].GetStaticHeight();
        else
            return pillarStructs[ptype].GetStaticHeight();
    }

    public void SetColliderActive(bool setActive)
    {
        pillarStructs[GV.PillarType.Ground].SetColliderActive(setActive);
    }

    public void SetDisturbed(bool _disturb)
    {
        //if (_disturb)
        //    Debug.Log("disturbed: " + _disturb);
        isDisturbed = _disturb;         
    }

    public void CleanPillar()
    {
        //toClean.pos.x = (int)MathHelper.RoundFloat(toClean.pos.x, 1);
        //if (toClean.pillarType == GV.PillarType.Ground)
        //    toClean.pos.y = (int)MathHelper.RoundFloat(toClean.pos.y, 1);
        //else
        //    toClean.pos.y = MathHelper.RoundFloat(toClean.pos.y, 1);
        //toClean.pos.z = (int)MathHelper.RoundFloat(toClean.pos.z, 1);
        //toClean.SetHeight(toClean.pos.y);

    }

    public float GetHeight()
    {
        return pos.y;
    } 
}




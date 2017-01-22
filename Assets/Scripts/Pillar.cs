using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour {

    [HideInInspector]
    public GV.PillarType pillarType;
    [HideInInspector]
    public Vector3 pos;
    public BoxCollider pillarCollider;
    Vector2 currentDir; //can be float, but will be int cast on use
    public bool DebugLogs = false;
    public List<GameObject> segments = new List<GameObject>();
    private int invisibleBelow;      //0 = grass, 1 = top, ..., 5 = bottom
    public bool isActive = true;
    public bool toDeactivate = false;
    public bool isStaticPillar = false;
    public bool isDisturbed = false;
    public Vector2 xypos { get { return new Vector2(pos.x, pos.z); }}

    public void Initialize(Vector3 _pos, GV.PillarType _pillarType)
    {
        pos = _pos;
        pillarType = _pillarType;
        SetHeight(pos.y);
        currentDir = new Vector2(0, 0);
        foreach (GameObject seg in segments)
            seg.SetActive(true);
        invisibleBelow = 5;  //nothing is invisible
        SkinPillar(pillarType);
    }

    public void SetIsActive(bool _isActive)
    {
        isActive = _isActive;
        gameObject.SetActive(_isActive);
    }

    /*
    public int GetInvisibleBelow()
    {
        return invisibleBelow;
    }
    */

    public void Disturb(bool _disturb)
    {

        if (isDisturbed != _disturb && pillarType == GV.PillarType.Water)
        {
            isDisturbed = _disturb;
            if (DebugLogs) Debug.Log("and changed disturb state: " + isDisturbed + " loc: " + MathHelper.V3toV2xz(pos));
            WorldGrid.Instance.waterManager.WasDisturbed(this, isDisturbed);
        }
        
        if (_disturb && !gameObject.activeInHierarchy)
        {
            if (DebugLogs) Debug.Log("disturb woke us up");
            SetIsActive(true);
        }
    }

    public void SetInvisibleBelow(int newInvBel)
    {
        if (newInvBel == invisibleBelow)
        {
            return;
        }
        else
        {
            for(int i = 0; i < segments.Count; i++)
            {
                if (i <= newInvBel)
                    segments[i].SetActive(true);
                else
                    segments[i].SetActive(false);
            }

        }/*
        else if (newInvBel > invisibleBelow)
        {
            for (int i = invisibleBelow + 1; i <= newInvBel; i++)
            {
                segments[i].SetActive(true);
            }
        }
        else
        {
            for (int j = invisibleBelow; j > newInvBel; j--)
            {
                segments[j].SetActive(false);
            }
        }*/
        invisibleBelow = newInvBel;
    }

    public int FindSegmentBelow(float minHeight)
    {
        return (int)((GetHeight() - minHeight) / 3) + 1;
        /*for (int i = 0; i <= 5; i++)
        {
            if (segments[i].transform.position.y + (transform.localScale.y/2) > minHeight)
                continue;
            if (i == 0)
                return i;
            return i - 1;
        }
        return 5;*/
    }

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

    private void SkinPillar(GV.PillarType pillarType)
    {
        //ground is already it by default
        if (pillarType == GV.PillarType.Water)
            foreach (GameObject segment in segments)
            {
                Renderer r = segment.GetComponent<Renderer>();
                if (r)
                {
                    r.material = Resources.Load("Materials/WaterMat", typeof(Material)) as Material;
                    r.receiveShadows = false;
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
    }

    public void DisturbAllNeighbors()
    {
        List<Pillar> neighs = WorldGrid.Instance.GetAllNeighbors(new Vector2(pos.x, pos.z));
        foreach (Pillar p in neighs)
            p.Disturb(true);
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

    public float GetHeight()
    {
        return pos.y;
    } 
}

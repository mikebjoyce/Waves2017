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

    /*
    public int GetInvisibleBelow()
    {
        return invisibleBelow;
    }
    */

    public void SetInvisibleBelow(int newInvBel)
    {
        if (newInvBel == invisibleBelow)
        {
            return;
        }
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
        }
        invisibleBelow = newInvBel;
    }

    public int FindSegmentBelow(float height)
    {
        for (int i = 0; i <= 5; i++)
        {
            if (segments[i].transform.position.y > height)
                continue;
            if (i == 0)
                return i;
            return i - 1;
        }
        return 5;
    }

    public void ModHeight(float modAmt)
    {
        SetHeight(pos.y + modAmt);
    }

    public void SetHeight(float newHeight)
    {
        Vector3 newPos = transform.position;
        newPos.y = newHeight;
        transform.position = newPos;
        pos.y = newHeight;
    }

    private void SkinPillar(GV.PillarType pillarType)
    {/*
        Renderer grass = transform.FindChild("Grass").GetComponent<Renderer>();
        Renderer mid = transform.FindChild("Middle").GetComponent<Renderer>();
        Renderer bot = transform.FindChild("Bottom").GetComponent<Renderer>();
        if (pillarType == GV.PillarType.Water)
        { //ground is already it by default
            top.material = Resources.Load("Materials/WaterMat", typeof(Material)) as Material;
            mid.material = Resources.Load("Materials/WaterMat", typeof(Material)) as Material;
            bot.material = Resources.Load("Materials/WaterMat", typeof(Material)) as Material;
        }*/
    }

    public Vector2 GetCurrent()
    {
        Vector2 normalizedCurrent = currentDir.normalized;
        if (Mathf.Abs(normalizedCurrent.x) > .5f && Mathf.Abs(normalizedCurrent.x) >= Mathf.Abs(normalizedCurrent.y))
            return new Vector2((int)(1 * Mathf.Sign(normalizedCurrent.x)), 0);
        if (Mathf.Abs(normalizedCurrent.y) > .5f && Mathf.Abs(normalizedCurrent.y) > Mathf.Abs(normalizedCurrent.x))
            return new Vector2(0, (int)(1 * Mathf.Sign(normalizedCurrent.y)));
        return new Vector2();
    }

    public void AddCurrent(Vector2 fromLoc, float strength)
    {
        currentDir += new Vector2(pos.x - fromLoc.x, pos.z - fromLoc.y) * strength;
        Debug_RotateFacingCurrent(GetCurrent());
    }

    public float GetHeight()
    {
        return pos.y;
    } 

    private void Debug_RotateFacingCurrent(Vector2 current)
    {
        if (DebugLogs)
            Debug.Log("current is: " + current);

        if (current == new Vector2())
        {
            transform.FindChild("Top").GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            transform.FindChild("Top").GetComponent<Renderer>().material.color = Color.white;
            if (current == new Vector2(1, 0))
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (current == new Vector2(-1, 0))
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (current == new Vector2(0, 1))
            {
                transform.eulerAngles = new Vector3(0, 270, 0);
            }
            else if (current == new Vector2(0, -1))
            {
                transform.eulerAngles = new Vector3(0, 90, 0);
            }
        }
    }
}

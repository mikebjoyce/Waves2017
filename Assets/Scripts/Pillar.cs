using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour {

    [HideInInspector]
    public GV.PillarType pillarType;
    [HideInInspector]
    public Vector3 pos;
    public BoxCollider pillarCollider;
    [HideInInspector]
    public Vector2 flowDir; //can be float, but will be int cast on use

    public void Initialize(Vector3 _pos, GV.PillarType _pillarType)
    {
        pos = _pos;
        pillarType = _pillarType;
        SetHeight(pos.y);
        flowDir = new Vector2(0, 1);
        SkinPillar(pillarType);
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
    {
        Renderer top = transform.FindChild("Top").GetComponent<Renderer>();
        Renderer mid = transform.FindChild("Middle").GetComponent<Renderer>();
        Renderer bot = transform.FindChild("Bottom").GetComponent<Renderer>();
        if (pillarType == GV.PillarType.Water)
        { //ground is already it by default
            top.material = Resources.Load("Materials/WaterMat", typeof(Material)) as Material;
            mid.material = Resources.Load("Materials/WaterMat", typeof(Material)) as Material;
            bot.material = Resources.Load("Materials/WaterMat", typeof(Material)) as Material;
        }
    }

    public float GetHeight()
    {
        return pos.y;
    } 
}

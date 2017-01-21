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

    public void Initialize(Vector3 _pos, GV.PillarType _pillarType)
    {
        pos = _pos;
        pillarType = _pillarType;
        SetHeight(pos.y);
        currentDir = new Vector2(0, 0);
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

    public float GetHeight()
    {
        return pos.y;
    } 
}

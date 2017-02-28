using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarUI : MonoBehaviour {

    public List<GameObject> segments; //set in editor
    public Collider coliBox;
    bool isVisible = true;
    float setHeight = 0;

    public void Initialize(GV.PillarType _ptype)
    {
        SkinPillar(_ptype);
    }

    public void SetColliderActive(bool setActive)
    {
        coliBox.enabled = setActive;
    }

    public void HeightModified(float newHeight)
    {
        setHeight = newHeight;
        if (isVisible)  //else no point
            gameObject.transform.position = new Vector3(transform.position.x, setHeight, transform.position.z);
        //camera update for neighbors and it
        //GV.gameFlow.cameraVisible.UpdateLocation(atLoc);
        //foreach (Vector2 dir in GV.Valid_Directions)
        //{
        //    if (GetPillarAt(atLoc + dir, true))
        //        cv.UpdateLocation(atLoc + dir);
        //}
    }

    public void SetVisible(bool setVisible)
    {
        gameObject.SetActive(setVisible);
        if (!isVisible && setVisible)  //wasnt visible before, we dont height modify, so need to now its visible
            HeightModified(setHeight);
        isVisible = setVisible;
        //Only used for water atm, If used for ground, would fuck with colliders and visible segement stuff
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

    /*
     private int invisibleBelow;      //0 = grass, 1 = top, ..., 5 = bottom
    public BoxCollider pillarCollider;
    public List<GameObject> segments = new List<GameObject>();
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

        }
        invisibleBelow = newInvBel;
    }

    public int FindSegmentBelow(float minHeight)
    {
        return (int)((GetHeight() - minHeight) / 3) + 1;
    }
    
    



     //foreach (GameObject seg in segments)    visible segements
        //    seg.SetActive(true);
        //invisibleBelow = 5;  //nothing is invisible
        //SkinPillar(pillarType);
    */


}

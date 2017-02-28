using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour {

    public GodsWhim god;
    public BoxCollider physicalCollider;
    public bool isHeld;

    public void Awake()
    {
        god = GameObject.FindObjectOfType<GodsWhim>();
    }

    public void SetHeld(bool setHeld)
    {
        isHeld = setHeld;
        god.holdingBook = setHeld;
        if(isHeld)
        {
            physicalCollider.isTrigger = true;
            transform.localPosition = new Vector3();
        }
        else
        {
            physicalCollider.isTrigger = false;
        }
    }

	public void Update()
    {
        if (!isHeld)
        {
            float heightBelow = WorldGrid.Instance.GetPillarStaticHeight(GridPos.ToGP(transform));
            if (transform.position.y < heightBelow)
                transform.position = new Vector3(transform.position.x, heightBelow, transform.position.z);
        }
        else
        {
            transform.localPosition = new Vector3();
            transform.localEulerAngles = new Vector3(-38, 0, 0);
        }
    }
}

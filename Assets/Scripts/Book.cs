using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour {

    public BoxCollider physicalCollider;
    public bool isHeld;

    public void SetHeld(bool setHeld)
    {
        isHeld = setHeld;
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
            float heightBelow = WorldGrid.Instance.GetHeightAt(MathHelper.V3toV2xz(transform.position));
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

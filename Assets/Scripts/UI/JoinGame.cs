using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinGame : MonoBehaviour {

    public List<PlayerSlot> slots = new List<PlayerSlot>();
    public int slotsActive;
    private int keyboardLeftID = -1;   // -1 == not assigned, 0 == first slot, ..., 3 == fourth slot
    private int keyboardRightID = -1;
    private int xbox1ID = -1;
    private int xbox2ID = -1;

    void Start ()
    {
        for(int i = 0; i < 4; i++)
        {
            slots[i].SetID(i + 1);
            slots[i].SetInputType(GV.InputType.None);
            slots[i].gameObject.SetActive(false);
            slotsActive = 0;
        }

	}
	
	void Update ()
    {
		
	}

    public void AddToList(GV.InputType inputType)
    {
        if (GetTypeID(inputType) == -1 && inputType != GV.InputType.None)
        {
            slots[slotsActive].SetInputType(inputType);
            slots[slotsActive].gameObject.SetActive(true);
            SetTypeID(inputType, slotsActive);
            slotsActive++;
        }
    }

    public void RemoveFromList(GV.InputType inputType)
    {
        for(int i=0; i<4; i++)
        {
            if (slots[i].GetInputType() != inputType)
            {
                continue;
            }
            else
            {
                SetTypeID(inputType, -1);
                //slots[i].SetInputType(GV.InputType.None);
                ShiftDown(i + 1);
                slotsActive--;
                return;
            }
        }
    }

    public void AddToListTEST(int inputType)
    {
        AddToList((GV.InputType)inputType);
        Debug.Log("Add: " + (GV.InputType)inputType);
        DebugButtons();
    }
    public void RemoveFromListTEST(int inputType)
    {
        RemoveFromList((GV.InputType)inputType);
        Debug.Log("Remove: " + (GV.InputType)inputType);
        DebugButtons();
    }
    public void DebugButtons()
    {
        Debug.Log("slots active: " + slotsActive + ", kL: " + keyboardLeftID + ", kR: " + keyboardRightID + ", x1: " + xbox1ID + ", x2: " + xbox2ID);
    }

    public void ShiftDown(int id)
    {
        for(int i = id; i < 4; i++)
        {
            if (slots[i].GetInputType() == GV.InputType.None)
            {
                slots[i - 1].SetInputType(GV.InputType.None);
                slots[i - 1].gameObject.SetActive(false);
            }
            else
            {
                slots[i - 1].SetInputType(slots[i].GetInputType());
                SetTypeID(slots[i].GetInputType(), GetTypeID(slots[i].GetInputType()) - 1);
                if (i==3)
                {
                    slots[i].SetInputType(GV.InputType.None);
                    slots[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public int GetTypeID(GV.InputType inputType)
    {
        switch (inputType)
        {
            case GV.InputType.KeyboardLeft:
                return keyboardLeftID;
            case GV.InputType.KeyboardRight:
                return keyboardRightID;
            case GV.InputType.Xbox1:
                return xbox1ID;
            case GV.InputType.Xbox2:
                return xbox2ID;
            default:
                return -9999;
        }
    }

    public void SetTypeID(GV.InputType inputType, int id)
    {
        switch (inputType)
        {
            case GV.InputType.KeyboardLeft:
                keyboardLeftID = id;
                break;
            case GV.InputType.KeyboardRight:
                keyboardRightID = id;
                break;
            case GV.InputType.Xbox1:
                xbox1ID = id;
                break;
            case GV.InputType.Xbox2:
                xbox2ID = id;
                break;
            default:
                break;
        }
    }
}

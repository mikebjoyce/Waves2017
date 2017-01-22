using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour {


    private int id;
    private GV.InputType inputType = GV.InputType.None;

    public Text idText;
    public Text nameText;

    void Start ()
    {

	}
	
	void Update ()
    {
		
	}

    public void SetID(int _id)
    {
        id = _id;
        idText.text = id.ToString();
    }

    public int GetID()
    {
        return id;
    }

    public void SetInputType(GV.InputType _it)
    {
        inputType = _it;
        switch (inputType)
        {
            case GV.InputType.KeyboardLeft:
                SetName("LEFT KEYBOARD");
                break;
            case GV.InputType.KeyboardRight:
                SetName("RIGHT KEYBOARD");
                break;
            case GV.InputType.Xbox1:
                SetName("XBOX CONTROLLER 1");
                break;
            case GV.InputType.Xbox2:
                SetName("XBOX CONTROLLER 2");
                break;
            case GV.InputType.None:
                SetName("");
                break;
            default:
                inputType = GV.InputType.None;
                SetName("");
                break;
        }
    }

    public GV.InputType GetInputType()
    {
        return inputType;
    }

    public void SetName(string str)
    {
        nameText.text = str;
    }

    public string GetName()
    {
        return nameText.text;
    }

    
}

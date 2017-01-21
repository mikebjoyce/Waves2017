using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper : MonoBehaviour {

	public static float RoundFloat(float toRound, int decimalPlaces)
    {
        float power = (int)Mathf.Pow(10, decimalPlaces);
        float roundedLarge = Mathf.Round(power * toRound);
        float result = roundedLarge / power;
        return result; 
    }
}

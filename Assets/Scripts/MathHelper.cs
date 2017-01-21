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

    public static Vector2 CastVectorToOffsetDir(Vector2 dir)
    {
        Vector2 normalizedDir = dir.normalized;
        if (Mathf.Abs(normalizedDir.x) > .5f && Mathf.Abs(normalizedDir.x) >= Mathf.Abs(normalizedDir.y))
            return new Vector2((int)(1 * Mathf.Sign(normalizedDir.x)), 0);
        if (Mathf.Abs(normalizedDir.y) > .5f && Mathf.Abs(normalizedDir.y) > Mathf.Abs(normalizedDir.x))
            return new Vector2(0, (int)(1 * Mathf.Sign(normalizedDir.y)));
        return new Vector2();
    }
}

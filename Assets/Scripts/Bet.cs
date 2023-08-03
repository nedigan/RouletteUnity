using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BetColor
{
    Red,
    Black,
    Green
}

public class Bet : MonoBehaviour
{
    public float MoneyPlaced = 0f;
    public BetColor Color;

    /// <summary>
    /// Adds the value to the total money placed on this bet.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A float of the new amount of money placed on this bet.</returns>
    public virtual float PlaceBetOfValue(float value)
    {      
        MoneyPlaced += value;
        return MoneyPlaced;
    }
}

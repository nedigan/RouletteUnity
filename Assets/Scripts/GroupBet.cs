using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupBet : Bet
{
    [SerializeField] private Bet[] _numbers;

    public override float PlaceBetOfValue(float value)
    {
        foreach (Bet number in _numbers)
        {
            number.PlaceBetOfValue(value / (float)_numbers.Length);
        }

        return 0;
    }
}

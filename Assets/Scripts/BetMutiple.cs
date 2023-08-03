using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// This class is used for the chip selections
public class BetMutiple : MonoBehaviour
{
    [SerializeField] private BetMultiples bet;
    [SerializeField] private PlaceBet _placeBet;
    public RectTransform _indicator;

    public void SelectBet()
    {
        _placeBet.SetBetMultiple((int)bet);
        _indicator.position = transform.position;
    }
}

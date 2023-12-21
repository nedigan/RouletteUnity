using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RouletteGame : MonoBehaviour
{
    [SerializeField] private PlaceBet _placeBet;
    [SerializeField] private Transform _noMoreBetsDisplay;
    [SerializeField] private Animator _displayAnimator;
    [SerializeField] private Animator _wheelAnimator;
    private Bet LastWinner;
    public bool RoundInPlay = false;

    [SerializeField] private Transform _offsetRotation;

    // Just stores all different offsets that relate to each number, definitely another way to do this, but fuk it
    const float _spacing = 9.729f;
    private Dictionary<int, float> numberOffsets = new Dictionary<int, float>() {
        { 0, 0f }, {26, _spacing}, {3, _spacing * 2}, {35, _spacing * 3}, {12, _spacing * 4}, {28, _spacing * 5}, {7, _spacing * 6}, {29, _spacing * 7},
        {18, _spacing * 8 }, {22, _spacing * 9}, {9, _spacing * 10}, {31, _spacing * 11}, {14, _spacing * 12}, {20, _spacing * 13}, {1, _spacing * 14}, {33, _spacing * 15},
        {16, _spacing * 16}, {24,_spacing * 17}, {5, _spacing * 18}, {10, _spacing * 19}, {23, _spacing * 20}, {8, _spacing * 21}, {30, _spacing * 22}, {11, _spacing * 23},
        {36, _spacing * 24 }, {13, _spacing * 25}, {27, _spacing * 26}, {6, _spacing * 27}, {34, _spacing * 28}, {17, _spacing * 29}, {25, _spacing * 30}, {2, _spacing * 31},
        {21, _spacing * 32}, {4, _spacing * 33}, {19, _spacing * 34}, {15, _spacing * 35}, {32, _spacing * 36}
    };

    private int _num = 0;

    private void Update()
    {
        //if (_animator.GetBool("EndRound"))
        //{
        //    EndRound();
        //    _animator.SetBool("EndRound", false);
        //}
    }
    public void StartRound()
    {
        // Wait for round to end
        if (RoundInPlay)
            return;

        RoundInPlay = true;

        _noMoreBetsDisplay.SetAsLastSibling();

        StartCoroutine(SpinBall());
    }

    private IEnumerator SpinBall()
    {
        
        _num = Random.Range(0, 37);

        // Rigged mode
        //_num = 2;

        // Display "no more bets"
        _displayAnimator.SetBool("ExpandIn", true);

        // Play spinning animation
        _offsetRotation.localRotation = Quaternion.Euler(0, 0, numberOffsets[_num]);
        _wheelAnimator.SetBool("spinning", true);

        // Wait for animation to finish
        yield return new WaitUntil(() => {
            return !_wheelAnimator.GetBool("spinning"); });

        EndRound();
    }

    // Is called from round behaviour script when the fade out animation has completed
    public void EndRound()
    {
        _displayAnimator.SetBool("ExpandIn", false);

        // Calculate winnings
        LastWinner = _placeBet.CalculateWinnings(_num);

        // have enough money, keep the same bets on the board
        if (_placeBet.Credit > _placeBet.LastBetAmount)
            _placeBet.Credit -= _placeBet.LastBetAmount;
        else
            _placeBet.ClearChipsAndBets(false); // Otherwise clear all bets 

        RoundInPlay = false;
    }

    public void ClearBets()
    {
        // Make sure round isnt in play
        if (!RoundInPlay && _placeBet != null)
            _placeBet.ClearChipsAndBets(true);
    }
}

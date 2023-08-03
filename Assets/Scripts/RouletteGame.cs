using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class RouletteGame : MonoBehaviour
{
    [SerializeField] private PlaceBet _placeBet;
    [SerializeField] private Transform _rolledNumber;
    [SerializeField] private Animator _animator;
    private TextMeshProUGUI _numberText;
    public Bet LastWinner;
    public bool RoundInPlay = false;

    private int _num = 0;

    private void Awake()
    {
        _numberText = _rolledNumber.gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (_animator.GetBool("EndRound"))
        {
            EndRound();
            _animator.SetBool("EndRound", false);
        }
    }
    public void StartRound()
    {
        // Wait for round to end
        if (RoundInPlay)
            return;

        RoundInPlay = true;
        _animator.SetBool("ExpandIn", RoundInPlay);
        _rolledNumber.SetAsLastSibling();

        StartCoroutine(SpinBall());
    }

    private IEnumerator SpinBall()
    {
        

        for (int i = 0; i < 10; i++) 
        {
            _num = Random.Range(0, 37);

            // Rigged mode
            //if (i == 9)
            //    _num = 20;

            _numberText.text = _num.ToString();
            Bet bet = _placeBet.NumToBet(_num);


            // TEMPORARY WAY OF CHANGING COLOR
            if (bet.Color == BetColor.Red)
                _numberText.color = Color.red;
            else if (bet.Color == BetColor.Black)
                _numberText.color = Color.black;
            else if (bet.Color == BetColor.Green)
                _numberText.color = Color.green;
            yield return new WaitForSecondsRealtime(0.5f);
        }

        LastWinner = _placeBet.CalculateWinnings(_num);

        // If you made money last bet and have enough money, keep the same bets on the board
        if (_placeBet.Credit > _placeBet.LastBetAmount)
            _placeBet.Credit -= _placeBet.LastBetAmount;
        else
            _placeBet.ClearChipsAndBets(true); // Otherwise clear all bets 

        // Give user time to read number
        yield return new WaitForSecondsRealtime(3);

        _animator.SetBool("ExpandIn", false);
    }

    // Is called from round behaviour script when the fade out animation has completed
    public void EndRound()
    {
        RoundInPlay = false;

        // Add other behaviour maybe?
    }
}

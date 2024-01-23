using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Timers;

public class RouletteGame : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlaceBet _placeBet;
    [SerializeField] private Transform _noMoreBetsDisplay;
    [SerializeField] private Animator _displayAnimator;
    [SerializeField] private Animator _wheelAnimator;
    [SerializeField] private CountDown _countDown;
    [SerializeField] private Transform _offsetRotation;
    [SerializeField] private TextMeshProUGUI _gameNumText;
    [SerializeField] private Database _database;

    [Header("Variables")]
    public bool RoundInPlay = false;

    private int _gameNum = 0;
    public int GameNum { get { return _gameNum; } 
        set 
        {
            _gameNum = value;
            _gameNumText.text = $"Game: {_gameNum}";
        } 
    }

    // Just stores all different offsets that relate to each number, definitely another way to do this, but fuk it
    private const float _spacing = 9.729f;
    private Dictionary<int, float> numberOffsets = new Dictionary<int, float>() {
        { 0, 0f }, {26, _spacing}, {3, _spacing * 2}, {35, _spacing * 3}, {12, _spacing * 4}, {28, _spacing * 5}, {7, _spacing * 6}, {29, _spacing * 7},
        {18, _spacing * 8 }, {22, _spacing * 9}, {9, _spacing * 10}, {31, _spacing * 11}, {14, _spacing * 12}, {20, _spacing * 13}, {1, _spacing * 14}, {33, _spacing * 15},
        {16, _spacing * 16}, {24,_spacing * 17}, {5, _spacing * 18}, {10, _spacing * 19}, {23, _spacing * 20}, {8, _spacing * 21}, {30, _spacing * 22}, {11, _spacing * 23},
        {36, _spacing * 24 }, {13, _spacing * 25}, {27, _spacing * 26}, {6, _spacing * 27}, {34, _spacing * 28}, {17, _spacing * 29}, {25, _spacing * 30}, {2, _spacing * 31},
        {21, _spacing * 32}, {4, _spacing * 33}, {19, _spacing * 34}, {15, _spacing * 35}, {32, _spacing * 36}
    };
    private Bet LastWinner;
    private int _num = 0;

    private void Start()
    {
        // Subscribe to countdown
        _countDown.onTimeIsUp.AddListener(StartRound);

        // Set game num to the amount of games, ensures that if crashes, game num will not go back to 0.
        StartCoroutine(_database.GetGameNumCount((count) =>
        {
            GameNum = count + 1;
        }));
    }
    public void StartRound()
    {
        // Wait for round to end
        if (RoundInPlay)
            return;

        RoundInPlay = true;

        // Put infront of everything
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

        // Wait for animation to finish - spinning parameter is set to false in animation behaviour script.
        yield return new WaitUntil(() => {
            return !_wheelAnimator.GetBool("spinning"); });

        EndRound();
    }

    // Is called from round behaviour script when the fade out animation has completed
    public void EndRound()
    {
        // Adds game to database
        _database.GameCompleted(_num, GameNum);

        // Plays fade out animation
        _displayAnimator.SetBool("ExpandIn", false);

        // Calculate winnings
       // LastWinner = _placeBet.CalculateWinnings(_num);
       //
       // // have enough money, keep the same bets on the board
       // if (_placeBet.Credit > _placeBet.LastBetAmount)
       //     _placeBet.Credit -= _placeBet.LastBetAmount;
       // else
       //     _placeBet.ClearChipsAndBets(false); // Otherwise clear all bets 

        // Round no longer in play
        RoundInPlay = false;

        // Increment Game Number
        GameNum++;

        // Start countdown again
        _countDown.ResetTimer(90); // 90 seconds
    }

    public void ClearBets()
    {
        // Make sure round isnt in play
        if (!RoundInPlay && _placeBet != null)
            _placeBet.ClearChipsAndBets(true);
    }

    public void ConfirmBets()
    {
        _database.ConfirmBets(GameNum, (int)_placeBet.Credit); 

        // Show printing status or something
    }
}

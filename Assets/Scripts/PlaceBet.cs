using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public enum BetMultiples
{
    One,
    Five,
    Ten,
    TwentyFive,
    Fifty
}
public class PlaceBet : MonoBehaviour
{
    private float _credit = 50f;
    public float Credit
    {
        get { return _credit; }
        set { _credit = Mathf.Round(value); }
    }
    public float LastBetAmount {  get; private set; }

    [SerializeField] private List<Bet> _bets;

    private float[] _betMultiples = new float[] { 1f, 5f, 10f, 25f, 50f };
    private Dictionary<Vector3, Chip> _chips = new Dictionary<Vector3, Chip>();   
    private float _betMultiple;
    public float BetMultiple { get { return _betMultiple; } }

    [SerializeField] private ContactFilter2D _contactFilter;
    public Vector2 BoxSize = Vector2.one;

    [Header("References")]
    [SerializeField] private GameObject _chipPrefab;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TextMeshProUGUI _creditText;
    [SerializeField] private RouletteGame _rouletteGame;

    private void Awake()
    {
        _betMultiple = _betMultiples[0];
        _creditText.text = $"Credit: {Credit}";
        LastBetAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // If clicks
        if (Input.GetMouseButtonDown(0))
        {
            if (_rouletteGame.RoundInPlay)
                return; 

            GameObject[] selectedObjects = GetSelectable();
            Chip chip;
            Vector2 chipPos;

            // Places chip and bet
            if (selectedObjects.Length > 0)
            {
                if (Credit >= BetMultiple)
                {
                    chipPos = GetMidPoint(selectedObjects);

                    if (!_chips.TryGetValue(chipPos, out chip))
                    {
                        chip = Instantiate(_chipPrefab).
                        GetComponent<Chip>();

                        // Places within canvas
                        chip.transform.SetParent(_canvas.transform, false);
                        chip.transform.position = chipPos; 

                        // Adds to dictionary
                        _chips[chipPos] = chip;
                    }

                    chip.IncrementValue(BetMultiple);
                    foreach (var obj in selectedObjects)
                    {
                        Bet bet = obj.GetComponent<Bet>();                        
                        bet.PlaceBetOfValue(BetMultiple / selectedObjects.Length);
                    }
                    Credit -= BetMultiple;
                    LastBetAmount += BetMultiple;
                }
            }
        }
        _creditText.text = $"Credit: {Credit}";
    }

    public Bet CalculateWinnings(int number)
    {
        Bet winningBet = NumToBet(number);
        float winnings = winningBet.MoneyPlaced * 36;
        Credit += winnings;
        return winningBet;     
    }

    /// <summary>
    /// Converts a number (int) into its Bet type.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public Bet NumToBet(int number)
    {
        foreach (Bet bet in _bets)
        {
            if (bet.gameObject.name == number.ToString())
            {
                return bet;
            }
        }

        Debug.LogError("Something went wrong");
        return null;
    }

    /// <summary>
    /// Clears chips and bets from the board.
    /// </summary>
    /// <param name="refund">
    /// Whether or not to refund the player.
    /// </param>
    public void ClearChipsAndBets(bool refund)
    {
        foreach (Chip chip in _chips.Values)
        {
            if (refund)
                Credit += chip.Value;

            Destroy(chip.gameObject);
        }
        _chips.Clear();

        foreach (Bet bet in _bets)
        {
            bet.MoneyPlaced = 0f;
        }

        LastBetAmount = 0f;
    }

    /// <summary>
    /// Gets the currently selected selectable component (bet)
    /// </summary>
    public GameObject[] GetSelectable()
    {
        List<GameObject> selected = new List<GameObject>();

        RaycastHit2D[] directHit = new RaycastHit2D[1];
        Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, _contactFilter, directHit, Mathf.Infinity);

        foreach (RaycastHit2D hit in directHit)
        {
            if (!hit)
                continue;
            GameObject hitGroup = hit.collider.gameObject;

            if (hitGroup.CompareTag("GroupSelector"))
            {
                return new GameObject[] { hitGroup };
            }
        }


        //--------------------------------
        // Used for number selections
        //--------------------------------

        RaycastHit2D[] hits = new RaycastHit2D[4]; //Max number of selections should be 4

        Physics2D.BoxCast(Camera.main.ScreenToWorldPoint(Input.mousePosition), BoxSize, 0f, Vector2.zero, _contactFilter, hits);

        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i])
                continue;

            GameObject number = hits[i].collider.gameObject;
            if (number.CompareTag("Number"))
                selected.Add(number);
        }
        return selected.ToArray();
    }
    
    public void SetBetMultiple(int betMultiple)
    {
        if (betMultiple >= 0 && betMultiple < _betMultiples.Length)
            _betMultiple = _betMultiples[betMultiple];
    }

    /// <summary>
    /// Gets appropriate midpoint by roulette layout
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    private Vector2 GetMidPoint(params GameObject[] points)
    {
        float x_pos = 0;
        float y_pos = 0;
        foreach(var point in points)
        {
            x_pos += point.transform.position.x;
            y_pos += point.transform.position.y;
        }
        x_pos /= points.Length;
        y_pos /= points.Length;

        return new Vector2(x_pos, y_pos);
    }
}

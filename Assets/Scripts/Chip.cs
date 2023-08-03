using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.ComponentModel.Design;

public class Chip : MonoBehaviour
{
    [SerializeField] private Sprite[] _chipImages;
    [SerializeField] private Image _chipImage;

    public float Value = 0;
    [SerializeField] private TextMeshProUGUI _text;
    public float IncrementValue(float value)
    {
        Value += value;
        _text.text = Value.ToString();

        if (Value < 5)
            _chipImage.sprite = _chipImages[(int)BetMultiples.One];
        else if (Value < 10)
            _chipImage.sprite = _chipImages[(int)BetMultiples.Five];
        else if (Value < 25)
            _chipImage.sprite = _chipImages[(int)BetMultiples.Ten];
        else if (Value < 50)
            _chipImage.sprite = _chipImages[(int)BetMultiples.TwentyFive];
        else
            _chipImage.sprite = _chipImages[(int)BetMultiples.Fifty];

        return Value;
    }
}

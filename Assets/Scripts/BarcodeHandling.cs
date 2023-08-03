using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Drawing;

public class BarcodeHandling : MonoBehaviour
{
    [SerializeField] private TMP_InputField _textInput;
    [SerializeField] private PlaceBet _placeBet;

    private bool _hasReadCode = false;

    private void Update()
    {
        if (!_textInput.isFocused)
            _textInput.ActivateInputField();

        if ((_textInput.text.Contains('\n') || _textInput.text.Contains('\r')) && !_hasReadCode)
        {
            _textInput.MoveTextStart(true);
            ReadCode();
        }
    }
    public void OnNewCode()
    {
        _hasReadCode = false;
    }

    public void ReadCode()
    {
        // Temporary way of adding credit from the code - just grab last digit
        _placeBet.Credit += float.Parse(_textInput.text[_textInput.text.Length - 2].ToString()); // -2 as last char is new line
        _hasReadCode = true;
    }
}

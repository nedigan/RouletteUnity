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
    [SerializeField] private Database _db;

    private bool _hasReadCode = false;
    public string _reqResult = null; // Is changed by database (_db) coroutines

    private void Update()
    {
        // Makes the text field always focussed
        if (!_textInput.isFocused)
            _textInput.ActivateInputField();

        // Barcode scanner always adds \n to end of code
        if ((_textInput.text.Contains("\n") || _textInput.text.Contains("\r")) && !_hasReadCode)
        {
            // Moves cursor to start whilst selecting text - so next code inputted overwrites previous
            _textInput.MoveTextStart(true);
            ReadCode();
        }
    }
    // Is ran when value is changed in text field
    public void OnNewCode()
    {
        _hasReadCode = false;
    }

    public void ReadCode()
    {
        // Temporary way of adding credit from the code - just grab last digit
        _hasReadCode = true;

        // Get ticket
        StartCoroutine(_db.GetTicketWithID(_textInput.text, (ticket) =>
        {
            _placeBet.Credit += ticket.credit;
        }));
    }
}

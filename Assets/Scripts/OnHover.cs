using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class OnHover : MonoBehaviour
{
    protected TextMeshProUGUI _text;
    private bool _hovering = false;
    private float _startFontSize;

    [SerializeField] private float _scaleMultiplier = 1.2f;
    private float _currentScale = 1f;

    private float _time = 0f;
    [SerializeField] private float _timeToFullScale = 0.2f;

    private void Awake()
    {
        SetUp();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        _time = Mathf.Clamp(_time, 0f, _timeToFullScale);
        _currentScale = Mathf.Lerp(1f, _scaleMultiplier, _time / _timeToFullScale);

        _text.fontSize = _currentScale * _startFontSize;
    }

    public void LateUpdate()
    {
        if (!_hovering)
            _time -= Time.deltaTime;
        _hovering = false;
    }

    public virtual void Hovering()
    {
        _hovering = true;
        _time += Time.deltaTime;
    }

    public virtual void SetUp()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _startFontSize = _text.fontSize;
    }
}

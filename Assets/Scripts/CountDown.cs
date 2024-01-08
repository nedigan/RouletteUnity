using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountDown : MonoBehaviour
{
    public float TimeLeft;
    public bool TimerOn = false;

    public UnityEvent onTimeIsUp;

    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        TimerOn = true;
    }

    private void Update()
    {
        if (TimerOn)
        {
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                _updateTimer(TimeLeft);
            }
            else
            {
                Debug.Log("Time is up");
                onTimeIsUp.Invoke();
                TimeLeft = 0;
                TimerOn = false;
            }
        }
    }

    private void _updateTimer(float currentTime)
    {
        // Add 1 so it doesnt show below 0
        currentTime += 1;
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        _text.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    public void ResetTimer(float time)
    {
        // Reset and start countdown again
        TimeLeft = time;
        _updateTimer(time);

        TimerOn = true;
    }
}

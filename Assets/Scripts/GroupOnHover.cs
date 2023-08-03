using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroupOnHover : OnHover
{
    public List<OnHover> nums;
    private bool _hasText = false;

    public override void Update()
    {
        if (_hasText)
            base.Update();
    }

    public override void Hovering()
    {
        // Hover all attached numbers
        foreach (var num in nums)
        {
            num.Hovering();
        }

        // If it has its own text, hover as well
        if (_hasText)
            base.Hovering();
    }

    public override void SetUp() 
    {
        _hasText = GetComponent<TextMeshProUGUI>() != null;
        if (_hasText) 
        { 
            base.SetUp();
        }
    }

}

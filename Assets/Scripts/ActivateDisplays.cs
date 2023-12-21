using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateDisplays : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // Check if additional displays are available and activate each.

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
            Display.displays[i].SetParams(Display.displays[i].systemWidth, Display.displays[i].systemHeight, 0,0);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

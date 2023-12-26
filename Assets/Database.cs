using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Database : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetTickets());   
    }

    IEnumerator GetTickets()
    {
        using (UnityWebRequest req = UnityWebRequest.Get("http://localhost/roulette/GetTickets.php"))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(req.error);
            }
            else
            {
                //Show results as dictionary
                Dictionary<string, object> jsonDict = JsonUtility.FromJson<Dictionary<string, object>>(req.downloadHandler.text);
                Debug.Log(jsonDict);
            }
        }
    }
}

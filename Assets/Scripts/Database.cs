using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Ticket
{
    public string id;
    public int credit;
}

[Serializable]
public class TicketsCollection
{
    public Ticket[] tickets;
}

public class Database : MonoBehaviour
{
    private string _url = "http://localhost/roulette/GetTickets.php";

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(GetAllTickets());   
    }

    IEnumerator GetAllTickets()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(_url))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(req.error);
            }
            else
            {
                //Show results as dictionary
                TicketsCollection table = JsonUtility.FromJson<TicketsCollection>(req.downloadHandler.text);
                Debug.Log(req.downloadHandler.text);
                Debug.Log(table.tickets[0].id);
                
            }
        }
    }
    public IEnumerator GetTicketWithID(string id, Action<Ticket> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(_url + "?id=" + id))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.responseCode == 400)
            {
                Debug.Log(req.error);
            }
            else
            {
                //Show results as collection
                TicketsCollection table = JsonUtility.FromJson<TicketsCollection>(req.downloadHandler.text);

                callback(table.tickets[0]); // ID is unqiue - should only return 1.

            }
        }
    }

}

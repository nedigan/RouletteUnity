using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.Windows;
using Mono.CompilerServices.SymbolWriter;

[Serializable]
public class Ticket
{
    public string id;
    public string bets;
    public int gamenum;
    public string date;
    public bool voucher;
    public int credit;
}

[Serializable]
public class TicketsCollection
{
    public Ticket[] tickets;
}

public class Database : MonoBehaviour
{
    private string _getURL = "http://localhost/roulette/GetTickets.php";
    private string _postURL = "http://localhost/roulette/PostTickets.php";
    private Bet[] _betCollection;

    [SerializeField] private PlaceBet _placeBet;

    // Start is called before the first frame update
    void Start()
    {
        _betCollection = _placeBet.BetCollection;
        //StartCoroutine(GetAllTickets());   
        //CreateUniqueIDNumberPlate();
    }

    IEnumerator GetAllTickets()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(_getURL))
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
                Debug.Log(SerializeToXml<TicketsCollection>(table));
                
            }
        }
    }
    public IEnumerator GetTicketWithID(string id, Action<Ticket> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(_getURL + "?id=" + id))
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

    public IEnumerator GetTicketCount(Action<int> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(_getURL + "?count"))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.responseCode == 400)
            {
                Debug.Log(req.error);
            }
            else
            {
                //Show results as collection
                callback(Int32.Parse(req.downloadHandler.text));
                   
            }
        }
    }

    public IEnumerator PostTicket(Ticket ticket)
    {
        string postData = JsonUtility.ToJson(ticket);
        Debug.Log(postData);

        using (UnityWebRequest req = UnityWebRequest.Post(_postURL, "")) // post data left blank as it is set after
        {
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(postData);
            req.uploadHandler = new UploadHandlerRaw(jsonBytes);
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.responseCode == 400)
            {
                Debug.Log(req.error);
            }
            else
            {
                //Show result
                Debug.Log(req.downloadHandler.text);

            }
        }
    }

    // Is run when clicking confirm bets button
    public void ConfirmBets()
    {
        // Add new database entry
        float[] bets = new float[37];

        for (int i = 0; i < _betCollection.Length; i++)
        {
            bets[i] = _betCollection[i].MoneyPlaced;
        }

        // Build ticket object
        Ticket ticket = new Ticket();
        CreateUniqueIDNumberPlate((ID) =>
        {
            ticket.id = ID;
            ticket.bets = SerializeToXml(bets);
            ticket.gamenum = 1; // TODO: testing - implement later
            ticket.date = DateTime.Now.ToString("yyyy-MM-dd");
            ticket.voucher = true; // TODO: testing - implement later
            ticket.credit = 50;
            StartCoroutine(PostTicket(ticket));
        }); 
    }

    public void CreateUniqueIDNumberPlate(Action<string> callback)
    {
        // AAA000 - start with this code
        // ZZZ999 - end with this code
        string ID = "";

        StartCoroutine(GetTicketCount((num) =>
        {
            num++;
            if (num < 0 || num > 175760999) // 26^3 * 999 + 26^2 * 999 + 26 * 999 + 999 = 175760999
            {
                throw new ArgumentOutOfRangeException("Input must be between 0 and 175760999");
            }



            int thousands = num / 1000;
            char[] chars = new char[3];

            chars[2] = (char)('A' + thousands % 26);
            thousands /= 26;
            chars[1] = (char)('A' + thousands % 26);
            thousands /= 26;
            chars[0] = (char)('A' + thousands % 26);

            ID = $"{new string(chars)}{num % 1000:D3}";

            callback(ID);
        }));
    }

    public static string SerializeToXml<T>(T value)
    {
        StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(writer, value);
        return writer.ToString();
    }
}
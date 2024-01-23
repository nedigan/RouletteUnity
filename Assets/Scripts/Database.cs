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
using System.Net.Http.Headers;
using System.Net.Sockets;

[Serializable]
public class Ticket
{
    // Fields in UPPERCASE due to sql table
    public string ID;
    public bool REDEEMED;
    public string BETS;
    public int GAMENUM;
    public string DATE;
    public bool VOUCHER;
    public int CREDIT;
}

[Serializable]
public class TicketsCollection
{
    public Ticket[] tickets;
}

[Serializable]
public class Game
{
    public int GAMENUM;
    public int WINNINGNUM;
}

public class Database : MonoBehaviour
{
    // Ticket URL's
    private string _getURL = "http://localhost/roulette/GetTickets.php";
    private string _postURL = "http://localhost/roulette/PostTickets.php";

    // Game URL
    private string _gameURL = "http://localhost/roulette/Games.php";

    private Bet[] _betCollection;

    [SerializeField] private PlaceBet _placeBet;
    [SerializeField] private Print _printer;

    // Start is called before the first frame update
    void Start()
    {
        _betCollection = _placeBet.BetCollection;
        //StartCoroutine(GetAllTickets());   
        //CreateUniqueIDNumberPlate();
       //StartCoroutine(GetWinningNum(1, (num) =>
       //{
       //    Debug.Log(num);
       //}));
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

    public IEnumerator GetGameNumCount(Action<int> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(_gameURL + "?count"))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.responseCode == 400)
            {
                Debug.Log(req.error);
            }
            else
            {
                callback(int.Parse(req.downloadHandler.text));
            }
        }
    }

    public IEnumerator GetWinningNum(int gameNum,  Action<int> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(_gameURL + "?gamenum=" + gameNum.ToString()))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.responseCode == 400)
            {
                Debug.Log(req.error);
            }
            else
            {
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

    public IEnumerator RedeemTicket(string ID)
    {
        string postData = ID.Trim('\n','\r');

        using (UnityWebRequest req = UnityWebRequest.Put(_postURL, postData)) // post data left blank as it is set after
        {
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

    public IEnumerator PostGame(Game game)
    {
        string postData = JsonUtility.ToJson(game);
        Debug.Log(postData);

        using (UnityWebRequest req = UnityWebRequest.Post(_gameURL, "")) // post data left blank as it is set after
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
    public void ConfirmBets(int gameNum, int remainingCredit)
    {
        // Add new database entry
        float[] bets = new float[37];

        bool betPlaced = false;

        for (int i = 0; i < _betCollection.Length; i++)
        {
            bets[i] = _betCollection[i].MoneyPlaced;

            if (_betCollection[i].MoneyPlaced > 0)
                betPlaced = true;
        }

        // Post/Print tickets
        StartCoroutine(GetTicketCount((count) =>
        {
            int offset = 1;
            // Build ticket object if a bet has been placed
            if (betPlaced)
            {
                Ticket ticket = new Ticket();

                // Bet ticket

                ticket.ID = CreateUniqueIDNumberPlate(count + offset);
                ticket.REDEEMED = true;
                ticket.BETS = SerializeToXml(bets);
                ticket.GAMENUM = gameNum;
                ticket.DATE = DateTime.Now.ToString("yyyy-MM-dd");
                ticket.VOUCHER = false; // TODO: testing - implement later
                ticket.CREDIT = 0;
                StartCoroutine(PostTicket(ticket));

                //TODO: Print ticket
                _printer.PrintTicket(ticket.ID, bets);

                // Update offset so voucher doesnt create duplicate
                offset++;
            }

            // Build voucher if there is remaning credit
            if (remainingCredit > 0)
            {
                Ticket voucher = new Ticket();
                // Voucher

                voucher.ID = CreateUniqueIDNumberPlate(count + offset);
                voucher.REDEEMED = true;
                voucher.BETS = "";
                voucher.GAMENUM = 0;
                voucher.DATE = DateTime.Now.ToString("yyyy-MM-dd");
                voucher.VOUCHER = true; // TODO: testing - implement later
                voucher.CREDIT = remainingCredit;
                StartCoroutine(PostTicket(voucher));

                _printer.PrintTabVoucher(voucher.ID, remainingCredit);

                offset++;
            }
        }));

        // Clear Bets and credit
        _placeBet.ClearChipsAndBets(false);
        _placeBet.Credit = 0;
    }

    public void GameCompleted(int winningNum, int gameNum) // game num is client side... could be wrong maybe? Something to consider
    {
        Game game = new Game();
        game.GAMENUM = gameNum;
        game.WINNINGNUM = winningNum;

        StartCoroutine(PostGame(game));
    }

    public string CreateUniqueIDNumberPlate(int num)
    {
        // AAA000 - start with this code
        // ZZZ999 - end with this code
        string ID = "";
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

        return ID;
    }

    public static string SerializeToXml<T>(T value)
    {
        StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(writer, value);
        return writer.ToString();
    }

    public static T DeserializeXml<T>(string xml)
    {
        StringReader reader = new StringReader(xml);

        XmlSerializer serializer = new XmlSerializer(typeof(T));

        return (T)serializer.Deserialize(reader);
    }
}
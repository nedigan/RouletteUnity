using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using UnityEngine;
using System.IO;

public class Print : MonoBehaviour
{
    private SerialPrinter _printer;
    // Start is called before the first frame update
    void Start()
    {
        _printer = new SerialPrinter(portName: "COM6", baudRate: 115200);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //PrintTest();
            PrintTabVoucher(80f);
        }
    }

    void PrintTest()
    {
        var e = new EPSON();
        _printer.Write( // or, if using and immediate printer, use await printer.WriteAsync
          ByteSplicer.Combine(
            e.CenterAlign(),
            e.PrintLine(""),
            e.SetBarcodeHeightInDots(360),
            e.SetBarWidth(BarWidth.Default),
            e.SetBarLabelPosition(BarLabelPrintPosition.None),
            e.PrintBarcode(BarcodeType.ITF, "0123456789"),
            e.PrintLine(""),
            e.PrintLine("B&H PHOTO & VIDEO"),
            e.PrintLine("420 NINTH AVE."),
            e.PrintLine("NEW YORK, NY 10001"),
            e.PrintLine("(212) 502-6380 - (800)947-9975"),
            e.SetStyles(PrintStyle.Underline),
            e.PrintLine("www.bhphotovideo.com"),
            e.SetStyles(PrintStyle.None),
            e.PrintLine(""),
            e.LeftAlign(),
            e.PrintLine("Order: 123456789        Date: 02/01/19"),
            e.PrintLine(""),
            e.PrintLine(""),
            e.SetStyles(PrintStyle.FontB),
            e.PrintLine("1   TRITON LOW-NOISE IN-LINE MICROPHONE PREAMP"),
            e.PrintLine("    TRFETHEAD/FETHEAD                        89.95         89.95"),
            e.PrintLine("----------------------------------------------------------------"),
            e.RightAlign(),
            e.PrintLine("SUBTOTAL         89.95"),
            e.PrintLine("Total Order:         89.95"),
            e.PrintLine("Total Payment:         89.95"),
            e.PrintLine(""),
            e.LeftAlign(),
            e.SetStyles(PrintStyle.Bold | PrintStyle.FontB),
            e.PrintLine("SOLD TO:                        SHIP TO:"),
            e.SetStyles(PrintStyle.FontB),
            e.PrintLine("  FIRSTN LASTNAME                 FIRSTN LASTNAME"),
            e.PrintLine("  123 FAKE ST.                    123 FAKE ST."),
            e.PrintLine("  DECATUR, IL 12345               DECATUR, IL 12345"),
            e.PrintLine("  (123)456-7890                   (123)456-7890"),
            e.PrintLine("  CUST: 87654321"),
            e.PrintLine(""),
            e.PrintLine(""),
            e.FullCutAfterFeed(5)
          )
        );
    }

    void PrintTabVoucher(float credit)
    {
        var e = new EPSON();
        _printer.Write( // or, if using and immediate printer, use await printer.WriteAsync
          ByteSplicer.Combine(
            e.CenterAlign(),
            e.PrintImage(File.ReadAllBytes("C:/Users/nedma/OneDrive/Documents/UnityProjects/Roulette/Assets/Images/TAB-Wagering-Logo.png"), true),
            e.PrintLine(""),
            e.SetStyles(PrintStyle.DoubleHeight),
            e.RightAlign(),
            e.PrintLine("VOUCHER               $50"),
            e.PrintLine(""),
            e.PrintLine(""),
            e.RightAlign(),
            e.SetStyles(PrintStyle.None),
            e.PrintLine("ACCRUED TOTAL            $50"),
            e.CenterAlign(),
            e.SetStyles(PrintStyle.Condensed),
            e.PrintLine("098 968 785 456 275 927"),
            e.CenterAlign(),
            e.PrintLine("THU 21SEP23 6:57:33 8697 000 001"),
            e.SetStyles(PrintStyle.None),
            e.PrintLine("TAB. WE'RE ON FOR RACING"),
            e.PrintLine("DOWNLOAD THE TAB APP"),
            e.PrintLine("GAMBLE RESPONSIBLY"),
            e.SetStyles(PrintStyle.Condensed),
            e.PrintLine("Tabcorp Wagering (VIC) Pty Ltd"),
            e.CenterAlign(),
            e.SetBarcodeHeightInDots(60),
            e.SetBarWidth(BarWidth.Default),
            e.SetBarLabelPosition(BarLabelPrintPosition.None),
            e.PrintBarcode(BarcodeType.CODE39, "POO123"),
            e.FullCutAfterFeed(5)
          )
        );
    }
}//poopoopoopoopooppop

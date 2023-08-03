using UnityEngine;
using ZXing;
using ZXing.Common;
public class CodeGenerator : MonoBehaviour
{
    private Texture2D _encoded;
    private Color32[] _generatedColorData;
    private IBarcodeWriter writer;

    private int num = 0;

    // Start is called before the first frame update
    void Start()
    {
        
        _encoded = new Texture2D(800, 500);
        _generatedColorData = new Color32[256 * 256];

        writer = new BarcodeWriter()
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = 500,
                Width = 800,
                PureBarcode = false,
                Margin = 10,
            },
        };


    }

    void Generate(string data)
    {
        _generatedColorData = writer.Write(data);
        _encoded.SetPixels32(_generatedColorData);
        _encoded.Apply();

        
    }

    // Update is called once per frame
    void Update()
    {
        //_generatedColorData = writer.Write("Winner: $20");
        //_encoded.SetPixels32(_generatedColorData);
        //_encoded.Apply();

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            num++;
            string code = num.ToString().PadLeft(8, '0');
            Generate(code);
            Debug.Log(code);
            
        }
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(10, 10, 200, 100), _encoded, ScaleMode.ScaleToFit);
    }
}

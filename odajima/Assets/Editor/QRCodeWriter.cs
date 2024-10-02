using System.IO;
using UnityEditor;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

public static class QRCodeWriter
{
    [MenuItem( "Tools/Create QRCode" )]
    private static void CreateQRCode()
    {
        // 保存する QR コードの画像ファイル名
        var path = Application.dataPath + "/Test.png";

        // QR コードを読み込んだ取得できるデータ
        var contents = "Test";

        // QR コードの画像の幅と高さ
        var width  = 256;
        var height = 256;

        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width  = width,
                Height = height
            }
        };

        var format  = TextureFormat.ARGB32;
        var texture = new Texture2D( width, height, format, false );
        var colors  = writer.Write( contents );

        texture.SetPixels32( colors );
        texture.Apply();

        using ( var stream = new FileStream( path, FileMode.OpenOrCreate ) )
        {
            var bytes = texture.EncodeToPNG();
            stream.Write( bytes, 0, bytes.Length );
        }

        AssetDatabase.Refresh();
    }
}
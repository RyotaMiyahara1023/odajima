using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using ZXing;

public sealed class QRCodeReader : MonoBehaviour
{
    private const string PERMISSION = Permission.Camera;

    public Text     m_text;
    public RawImage m_rawImage;

    private WebCamTexture m_webCamTexture;

    private void Awake()
    {
        // カメラの使用許可をリクエストします
        Permission.RequestUserPermission( PERMISSION );
    }

    private void Update()
    {
        // まだカメラの準備ができておらず
        if ( m_webCamTexture == null )
        {
            // カメラの使用が許可された場合
            if ( Permission.HasUserAuthorizedPermission( PERMISSION) )
            {
                // カメラを準備して
                var width  = Screen.width;
                var height = Screen.height;

                m_webCamTexture = new WebCamTexture( width, height );

                // カメラの使用を開始します
                m_webCamTexture.Play();

                // カメラが写している画像をゲーム画面に表示します
                m_rawImage.texture = m_webCamTexture;
            }
        }
        else
        {
            // カメラが写している QR コードからデータを取得して
            // ゲーム画面に表示します
            m_text.text = Read( m_webCamTexture );
        }
    }

    private static string Read( WebCamTexture texture )
    {
        var reader = new BarcodeReader();
        var rawRGB = texture.GetPixels32();
        var width  = texture.width;
        var height = texture.height;
        var result = reader.Decode( rawRGB, width, height );

        return result != null ? result.Text : string.Empty;
    }
}
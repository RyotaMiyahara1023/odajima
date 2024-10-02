using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using ZXing;

public class WebCam : MonoBehaviour
{
    public RawImage RawImage;
    WebCamTexture webCam;
    public Text     m_text;
    public GameObject     panel;
    public CameraGyro     cameraGyro;
    string text_qr;
    public bool ar = false;
    public GameObject     Read_QR;

    void Start()
    {   /*
        // WebCamTextureのインスタンスを生成
        webCam = new WebCamTexture();
        // RawImageのテクスチャにWebCamTextureのインスタンスを設定
        rawImage.texture = webCam;
        Vector3 angles = rawImage.GetComponent<RectTransform>().eulerAngles;
        angles.z = -90;

        rawImage.GetComponent<RectTransform>().eulerAngles = angles;
        // カメラ表示開始
        webCam.Play();
        */

        // WebCamTextureのインスタンスを生成
        webCam = new WebCamTexture();
        
        //RawImageのテクスチャにWebCamTextureのインスタンスを設定
        RawImage.texture = webCam;

        Vector3 angles = RawImage.GetComponent<RectTransform>().eulerAngles;
        angles.z = -90;

        RawImage.GetComponent<RectTransform>().eulerAngles = angles;

        //縦横のサイズを要求
        webCam.requestedWidth = 1080;
        webCam.requestedHeight = 1920;

        //カメラ表示開始
        webCam.Play();

        float scale;
        Vector2 size;
        if (webCam.width > webCam.height) {
            // 横長映像 横のサイズに合わせる
            size = RawImage.GetComponent<RectTransform>().sizeDelta;
            scale = size.x / webCam.width;
            size.y = webCam.height * scale;
            RawImage.GetComponent<RectTransform>().sizeDelta = size;
        } else {
            // 縦長映像 縦のサイズに合わせる
            size = RawImage.GetComponent<RectTransform>().sizeDelta;
            scale = size.y / webCam.height;
            size.x = webCam.width * scale;
            RawImage.GetComponent<RectTransform>().sizeDelta = size;
        }

        #if UNITY_IOS
            Vector3 scaletmp = RawImage.GetComponent<RectTransform>().localScale;
            scaletmp.y *= -1;
            RawImage.GetComponent<RectTransform>().localScale = scaletmp;
        #endif

        cameraGyro = GameObject.Find("Main Camera").GetComponent<CameraGyro>();
    }

    private void Update()
    {
        //m_text.text = Read( webCam );   
        if(!ar) Read_QR.SetActive(true);
        else Read_QR.SetActive(false);
    }

    public void ReadQR(){

        //m_text.text = Read( webCam );
        
        ar = true;

        text_qr = Read( webCam );

        //if (text_qr.Equals("Test"))
        //{
            //m_text.text = Read( webCam );
            panel.SetActive(true);
        //}
    }

    public void Start_AR(){
        panel.SetActive(false);
        //Invoke("cameraGyro." + (text_qr).ToString() + "()",1f);
        cameraGyro.Test();
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
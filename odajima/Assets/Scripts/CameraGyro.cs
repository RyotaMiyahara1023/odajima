using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraGyro : MonoBehaviour
{
    public GameObject gy;
    public GameObject test;
    public GameObject cube;
    public GameObject Explanation;
    public WebCam webcam;
    
#if UNITY_ANDROID

    Transform m_transform;
 
    // 調整値
    //readonly Quaternion _BASE_ROTATION = Quaternion.Euler(90, 0, 0);

    double lastCompassUpdateTime = 0;
    Quaternion correction = Quaternion.identity;
    Quaternion targetCorrection = Quaternion.identity;
 
    // Androidの場合はScreen.orientationに応じてrawVectorの軸を変換
    static Vector3 compassRawVector
    {
        get
        {
            Vector3 ret = Input.compass.rawVector;
             
            if(Application.platform == RuntimePlatform.Android)
            {
                switch(Screen.orientation)
                {
                    case ScreenOrientation.LandscapeLeft:
                        ret = new Vector3(-ret.y, ret.x, ret.z);
                        break;
                    case ScreenOrientation.LandscapeRight:
                        ret = new Vector3(ret.y, -ret.x, ret.z);
                        break;
                    case ScreenOrientation.PortraitUpsideDown:
                        ret = new Vector3(-ret.x, -ret.y, ret.z);
                        break;
                }
            }
             
            return ret;
        }
    }
     
    // Quaternionの各要素がNaNもしくはInfinityかどうかチェック
    static bool isNaN(Quaternion q)
    {
        bool ret = 
            float.IsNaN(q.x) || float.IsNaN(q.y) || 
            float.IsNaN(q.z) || float.IsNaN(q.w) || 
            float.IsInfinity(q.x) || float.IsInfinity(q.y) || 
            float.IsInfinity(q.z) || float.IsInfinity(q.w);
         
        return ret;
    }
     
    static Quaternion changeAxis(Quaternion q)
    {
        return new Quaternion(-q.x, -q.y, q.z, q.w);
    }

    void Start()
    {
        Input.gyro.enabled = true;
        //m_transform = transform;
        webcam = GameObject.Find("Main Camera").GetComponent<WebCam>();

        Input.compass.enabled = true;
    }

    void Update()
    {
        /*
        transform.rotation = Quaternion.Euler(0, 0, -180) * Quaternion.Euler(-90, 0, 0) * Input.gyro.attitude * Quaternion.Euler(0, 0, 180);
        Text gy_text = gy.GetComponent<Text>();
        
        gy_text.text = "x : " + transform.rotation.x.ToString() + "\ny : " + transform.rotation.z.ToString() + "\nz : " + transform.rotation.y.ToString();
        */
        // ジャイロの値を獲得する
        /*Quaternion gyro = Input.gyro.attitude;
 
        // 自信の回転をジャイロを元に調整して設定する
        m_transform.localRotation = _BASE_ROTATION * (new Quaternion(-gyro.x, -gyro.y, gyro.z, gyro.w));
        */
            Quaternion gorientation = changeAxis(Input.gyro.attitude);
 
            if (Input.compass.timestamp > lastCompassUpdateTime)
            {
                lastCompassUpdateTime = Input.compass.timestamp;
 
                Vector3 gravity = Input.gyro.gravity.normalized;
                Vector3 rawvector = compassRawVector;
                Vector3 flatnorth = rawvector - 
                    Vector3.Dot(gravity, rawvector) * gravity;
  
                Quaternion corientation = changeAxis(Quaternion.Inverse(Quaternion.LookRotation(flatnorth, -gravity)));
  
                // +zを北にするためQuaternion.Euler(0,0,180)を入れる。
                Quaternion tcorrection = corientation * Quaternion.Inverse(gorientation) * Quaternion.Euler(0, 0, 180);
 
                // 計算結果が異常値になったらエラー
                // そうでない場合のみtargetCorrectionを更新する。
                if(!isNaN(tcorrection)) targetCorrection = tcorrection;
            }

            if (Quaternion.Angle(correction, targetCorrection) < 360)
            {
                correction = Quaternion.Slerp(
                    correction, targetCorrection, 0.02f);
            }
            else correction = targetCorrection;

            var ro = correction * gorientation;
 
            transform.localRotation = correction * gorientation;
        

        Text gy_text = gy.GetComponent<Text>();
        
        //gy_text.text = "x : " + (-gyro.x).ToString() + "\ny : " + (-gyro.y).ToString() + "\nz : " + (-gyro.z).ToString();
        gy_text.text = "x : " + (transform.localEulerAngles.x).ToString("f0") + "\ny : " + (transform.localEulerAngles.y).ToString("f0") + "\nz : " + (transform.localEulerAngles.z).ToString("f0");

         
    }
#elif UNITY_IOS    
    void Start()
    {
        webcam = GameObject.Find("Main Camera").GetComponent<WebCam>();

        //Input.compass.enabled = true;
    }

    private void Update()
    {
        var rotRH = Input.gyro.attitude;
        //var rot = new Quaternion(-rotRH.x, -rotRH.z, -rotRH.y, rotRH.w) * Quaternion.Euler(90f, 0f, 0f);

        var rot = (new Quaternion(-rotRH.x, -rotRH.z, -rotRH.y, rotRH.w)) * Quaternion.Euler(90f, 0f, 0f);

        transform.localRotation = rot;

        //transform.localRotation = rot;

        Text gy_text = gy.GetComponent<Text>();
        gy_text.text = "x : " + (transform.localEulerAngles.x).ToString("f0") + "\ny : " + (transform.localEulerAngles.y).ToString("f0") + "\nz : " + (transform.localEulerAngles.z).ToString("f0");
    }
#endif

    public void Test(){
        Text test_text = test.GetComponent<Text>();
        //GameObject obj= Instantiate(cube, new Vector3( 8, 0.0f, 0), Quaternion.identity);
        //Instantiate(cube, new Vector3( 8*(transform.rotation.x), 0.0f, 8*transform.rotation.y), Quaternion.identity);
        test_text.text += "(" + 8 + ","  + 0 + ","  + 0 + ")\n";
        StartCoroutine("Wait");
    }

     IEnumerator Wait()
    {
        Explanation.SetActive(true);
        
        //GameObject obj= Instantiate(cube, new Vector3( -8*Mathf.Sin((transform.localEulerAngles.y/(2 * Mathf.PI))), 0.0f, -8*Mathf.Cos((transform.localEulerAngles.y/(2 * Mathf.PI)))), Quaternion.identity);
        //GameObject obj= Instantiate(cube, new Vector3( -8*Mathf.Sin((transform.localEulerAngles.y/180)), 0.0f, -8*Mathf.Cos((transform.localEulerAngles.y/180))), Quaternion.identity);
        //GameObject obj= Instantiate(cube, new Vector3( -8*Mathf.Sin(((transform.localEulerAngles.y/180.0f)*Mathf.PI)), 0.0f, -8*Mathf.Cos(((transform.localEulerAngles.y/180.0f)*Mathf.PI))), Quaternion.identity);
        GameObject obj= Instantiate(cube, new Vector3( -80*Mathf.Sin(((transform.localEulerAngles.y/180.0f)*Mathf.PI)) - 8f, -11.5f, -80*Mathf.Cos(((transform.localEulerAngles.y/180.0f)*Mathf.PI)) + 2.8f), Quaternion.identity);
        //GameObject obj= Instantiate(cube, new Vector3(0.0f, 0.0f, -8.0f), Quaternion.identity);
        
        yield return new WaitForSeconds(5);
    
        Explanation.SetActive(false);
        
        Destroy (obj);

        webcam.ar = false;
    }
}
using UnityEngine;
using System.IO.Ports;
using System;

public class SerialController : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM4", 115200); // COMポートとボーレートは環境に合わせて変更
    getPoint point;
    public ReceiveData receiveData;

    private Vector3 newPosition;
    private Vector3 previousPosition; // 前回の深度データ

    private float pz = 0.6f, nz = 0; // previous, new
    private float diffZ;

    private int pulse_width;
    public int MAX_PULSEWIDTH;
    public int MIN_PULSEWIDTH;
    public float a;   // 1pulseあたりの移動量
    private float interval = 11f;
    public float zmove; // これ以下なら動かさない

    //private const int updateInterval = 11;  // 例えば100msごとに更新
    //private float timeSinceLastUpdate = 0f;  // 最後に更新された時間

    public bool doTracking=false;

    public bool move=false;
    private int pulse;
    private int count=0;
    private int previousW;
    private int newW;
    private int deltaW;
    public int i,n;
    private int skipFrame=0;
    private int n_decceleration;
    private float stopCount=0;

    void Start()
    {
        serialPort.Open();
        //GameObject obj=GameObject.Find("Nymph Fairy");
        //point=obj.GetComponent<getPoint>();
        Debug.Log("Serial Port Opened");
    }

    void Update()
    {

        // 深度データを取得
        newPosition = receiveData.sendPosition();

        pz = nz;
        nz = newPosition.z;

        diffZ = nz - pz;
        if(Math.Abs(diffZ)>0.05){
            doTracking=true;
        }else{
            doTracking=false;
        }

        Debug.Log(doTracking);

        // データ取得後の処理
        ProcessPulseWidth();

        // 例外処理
        //CalculationException();

        if(skipFrame<3){
            skipFrame++;
            return;
        }


        if(receiveData.adjust){
            Debug.Log("stop");
            if(serialPort.IsOpen){
                pulse_width=2000;
                string data = pulse_width.ToString();
                serialPort.WriteLine(data);
            }
            return;
        }else{
            Debug.Log("go");
            if(serialPort.IsOpen){
                if (Math.Abs(pulse_width) < 2000 && Math.Abs(pulse_width) > 10)
                {
                    string data = pulse_width.ToString();
                    serialPort.WriteLine(data);
                    Debug.Log(pulse_width);
                }
            }
        }
   
    }

    float deltaW2;
    void ProcessPulseWidth()
    { 
        // 深度がゼロでない場合にパルス幅を計算
        if (doTracking)
        {
            previousW=pulse_width;
            // パルス幅を計算（移動量に基づいて）
            pulse_width = (int)(a * 500 * interval / diffZ);
            /*
            if (Math.Abs(diffZ) <= zmove){
                stopCount+=Time.deltaTime;
                if(stopCount==450){
                
                }
            }*/

            newW=pulse_width;
            deltaW=newW-previousW;
            if (doTracking) i = 0;
            //UnityEngine.Debug.Log("IEqualsZero:pulse_width = "+pulse_width+", delta_t = "+delta_t+", delta_z = "+delta_z+", z_i = "+z_i+", z_imin1 = "+z_imin1+", tracking = "+TrackingDone);
            doTracking = false;
        }

        if (i <= n)
        {
            if (deltaW == 0) pulse_width = newW;
            else if (previousW*newW > 0)
            {
                pulse_width = (int)(previousW + (float)(deltaW*i)/ (float)n);
            }
            else if (previousW*newW < 0)
            {
                if (previousW == MAX_PULSEWIDTH)
                {
                    previousW = -MAX_PULSEWIDTH;
                    deltaW = newW - previousW;
                    pulse_width = (int)(previousW + (float)(deltaW*i)/ (float)(n));
                }
                else if (newW == MAX_PULSEWIDTH)
                {
                    newW = -MAX_PULSEWIDTH;
                    deltaW = newW - previousW;
                    pulse_width = (int)(previousW + (float)(deltaW*i)/ (float)(n));
                }
                else
                {
                    if (i <= n/2) {
                        deltaW2 = (previousW > 0) ? MAX_PULSEWIDTH - previousW : -MAX_PULSEWIDTH - previousW;
                        pulse_width = (int)(previousW + (deltaW2*i) / (int)(n/2));
                    } else {
                        deltaW2 = (newW > 0) ? MAX_PULSEWIDTH - newW : -MAX_PULSEWIDTH - newW;
                        pulse_width = (int)(newW + (deltaW2*(n - i))/ (int)(n/2));
                    }
                }
            }
        } else
        {
            pulse_width = newW;
        }
        ++i;
    }

    void CalculationException()
    {  
        // 目標位置に近い場合はパルス幅を固定
        if (nz < 0.4 || nz > 0.8)
        {
            ExceptionInOutOfRecognition();
            /*
            pulse_width = 2000;
            string data = pulse_width.ToString();
            serialPort.WriteLine(data);
            //point.t=false;
            return;*/
        }
    }


    private bool OnTheWayOfOutOfRecognitionExecution = false;
    private void ExceptionInOutOfRecognition()
    {
        if (OnTheWayOfOutOfRecognitionExecution) return;
        previousW = pulse_width;
        newW = MAX_PULSEWIDTH;
        deltaW = newW - previousW;
        pz = nz;
        diffZ = 0f;
        i = 0;
        n = N_Decceleration();

        OnTheWayOfOutOfRecognitionExecution = true;
        //UnityEngine.Debug.Log("Inactive, n = "+n);
    }

    private int N_Decceleration() {
        float w = Mathf.Abs(pulse_width);
        n_decceleration = (int)(
            ( (MIN_PULSEWIDTH - MAX_PULSEWIDTH) / (MAX_PULSEWIDTH - MIN_PULSEWIDTH) )*(w - MAX_PULSEWIDTH) + MIN_PULSEWIDTH
        );
        return n_decceleration;
    }


    void OnApplicationQuit()
    {
        serialPort.Close();
        Debug.Log("Serial Port Closed");
    }
}

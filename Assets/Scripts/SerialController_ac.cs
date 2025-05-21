using UnityEngine;
using System.IO.Ports;
using System;

public class SerialController_ac : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM4", 115200); // COMポートとボーレートは環境に合わせて変更
    //getPoint point;
    colorChanged point;
    public ReceiveData receiveData_ac;

    private Vector3 newPosition;
    private Vector3 previousPosition; // 前回の深度データ

    private float pz = 0.6f, nz = 0; // previous, new
    private float diffZ;

    private int pulse_width;
    public int MAX_PULSEWIDTH_AC;
    public int MIN_PULSEWIDTH_AC;
    public float a_ac;   // 1pulseあたりの移動量
    private float interval = 11f;
    public float zmove_ac; // これ以下なら動かさない

    //private const int updateInterval = 11;  // 例えば100msごとに更新
    //private float timeSinceLastUpdate = 0f;  // 最後に更新された時間

    public bool doTracking_ac=false;

    public bool move_ac=false;
    private int pulse;
    private int count=0;
    private int previousW;
    private int newW;
    private int deltaW;
    public int i_ac,n_ac;
    private int skipFrame=0;
    private int n_decceleration;
    private float stopCount=0;
    private int count_yes=0;

    void Start()
    {
        serialPort.Open();
        GameObject obj=GameObject.Find("Cube");
        point=obj.GetComponent<colorChanged>();
        Debug.Log("Serial Port Opened");
    }

    void Update()
    {

        // 深度データを取得
        newPosition = receiveData_ac.sendPosition();

        pz = nz;
        nz = newPosition.z;

        diffZ = nz - pz;
        //Debug.Log(diffZ);
        if(Math.Abs(diffZ)>0.005)
        {   count_yes++;
            stopCount=0;
            if(count_yes>=2){
                doTracking_ac=true;
                Debug.Log("yes");
            }
        
        }else{
            doTracking_ac=false;
            stopCount++;
        }

        // データ取得後の処理
        ProcessPulseWidth();

        // 例外処理
        //CalculationException();
        
        /*
        if (serialPort.IsOpen)
        {
            if (Math.Abs(pulse_width) < 2000 && Math.Abs(pulse_width) > 10)
            {
                Debug.Log(pulse_width);
                string data = pulse_width.ToString();
                serialPort.WriteLine(data);
            }
        }*/

        if(skipFrame<3){
            skipFrame++;
            return;
        }

        Debug.Log(pulse_width);

        if(serialPort.IsOpen){
            /*if(stopCount>=5){
                pulse_width=2000;
                string data = pulse_width.ToString();
                serialPort.WriteLine(data);
                Debug.Log("yes");
            }
            else*/ 
            if (Math.Abs(pulse_width) <= 2000 && Math.Abs(pulse_width) > 10)
            {
                //Debug.Log(pulse_width);
                string data = pulse_width.ToString();
                serialPort.WriteLine(data);
            }
        }
   
    }

    float deltaW2;
    void ProcessPulseWidth()
    { 
        // 深度がゼロでない場合にパルス幅を計算
        if (doTracking_ac)
        {
            previousW=pulse_width;
            // パルス幅を計算（移動量に基づいて）
            pulse_width = (int)(a_ac * 500 * interval / diffZ);
            /*
            if (Math.Abs(diffZ) <= zmove_ac){
                stopCount+=Time.deltaTime;
                if(stopCount==450){
                
                }
            }*/

            newW=pulse_width;
            deltaW=newW-previousW;
            if (doTracking_ac) i_ac = 0;
            //UnityEngine.Debug.Log("IEqualsZero:pulse_width = "+pulse_width+", delta_t = "+delta_t+", delta_z = "+delta_z+", z_i = "+z_i+", z_imin1 = "+z_imin1+", tracking = "+TrackingDone);
            doTracking_ac = false;
        }

        if(i_ac>100){
            pulse_width=2000;
        }else 
        if (i_ac <= n_ac)
        {
            if (deltaW == 0) pulse_width = newW;
            else if (previousW*newW > 0)
            {
                pulse_width = (int)(previousW + (float)(deltaW*i_ac)/ (float)n_ac);
            }
            else if (previousW*newW < 0)
            {
                if (previousW == MAX_PULSEWIDTH_AC)
                {
                    previousW = -MAX_PULSEWIDTH_AC;
                    deltaW = newW - previousW;
                    pulse_width = (int)(previousW + (float)(deltaW*i_ac)/ (float)(n_ac));
                }
                else if (newW == MAX_PULSEWIDTH_AC)
                {
                    newW = -MAX_PULSEWIDTH_AC;
                    deltaW = newW - previousW;
                    pulse_width = (int)(previousW + (float)(deltaW*i_ac)/ (float)(n_ac));
                }
                else
                {
                    if (i_ac <= n_ac/2) {
                        deltaW2 = (previousW > 0) ? MAX_PULSEWIDTH_AC - previousW : -MAX_PULSEWIDTH_AC - previousW;
                        pulse_width = (int)(previousW + (deltaW2*i_ac) / (int)(n_ac/2));
                    } else {
                        deltaW2 = (newW > 0) ? MAX_PULSEWIDTH_AC - newW : -MAX_PULSEWIDTH_AC - newW;
                        pulse_width = (int)(newW + (deltaW2*(n_ac - i_ac))/ (int)(n_ac/2));
                    }
                }
            }
        } else
        {
            pulse_width = newW;
        }
        ++i_ac;
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
        newW = MAX_PULSEWIDTH_AC;
        deltaW = newW - previousW;
        pz = nz;
        diffZ = 0f;
        i_ac = 0;
        n_ac = N_Decceleration();

        OnTheWayOfOutOfRecognitionExecution = true;
        //UnityEngine.Debug.Log("Inactive, n_ac = "+n_ac);
    }

    private int N_Decceleration() {
        float w = Mathf.Abs(pulse_width);
        n_decceleration = (int)(
            ( (MIN_PULSEWIDTH_AC - MAX_PULSEWIDTH_AC) / (MAX_PULSEWIDTH_AC - MIN_PULSEWIDTH_AC) )*(w - MAX_PULSEWIDTH_AC) + MIN_PULSEWIDTH_AC
        );
        return n_decceleration;
    }


    void OnApplicationQuit()
    {
        serialPort.Close();
        Debug.Log("Serial Port Closed");
    }
}

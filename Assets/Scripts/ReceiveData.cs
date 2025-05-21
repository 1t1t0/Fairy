using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ReceiveData : MonoBehaviour
{
    UdpClient udpClient;
    IPEndPoint remoteEndPoint;

    public Vector3 position;

    // getPointスクリプトへの参照
    public getPoint pointScript;
    //public colorChanged color;
    public bool adjust=true; 

    public float x,y,z;
    //public float xscale, yscale;

    void Start()
    {
        udpClient = new UdpClient(12345);  // Python側で指定したポート
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 12345);
    }

    void Update()
    {
        /*
        if(adjust){
            pointScript.adjustPos();
            if(Input.GetKeyDown(KeyCode.A)){
                adjust=false;
            }
        }else{*/
        if (udpClient.Available > 0)
        {
            byte[] data = udpClient.Receive(ref remoteEndPoint);

            // 受信したデータを浮動小数点に変換 (x, y, z)
            x = BitConverter.ToSingle(data, 0);
            y = BitConverter.ToSingle(data, 4);
            z = BitConverter.ToSingle(data, 8); 

            //Debug.Log($"Received data - X: {x}, Y: {y}, Z: {z}");

            if(adjust){
                pointScript.adjustPos();
            }else{
                pointScript.UpdatePosition(x, y, z);
            }
            //color.UpdateP(x,y,z);
        }
        if(Input.GetKeyDown(KeyCode.A)){
            adjust= !adjust;
        }
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }

    public Vector3 sendPosition(){
        position= new Vector3(x,y,z);   
        return position;
    }
}

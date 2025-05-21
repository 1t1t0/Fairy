using UnityEngine;
using System.IO.Ports;

public class SerialHandler : MonoBehaviour
{
    public string portName = "COM3"; // 使用するポート名を設定
    public int baudRate = 9600;      // ボーレートを設定
    private SerialPort serialPort;

    void Start()
    {
        Open();
    }

    void Update()
    {
        // Aボタンが押されたときにArduinoにデータを送信
        if (Input.GetKeyDown(KeyCode.A))
        {
            Write("A");
            Debug.Log("Sent 'A' to Arduino");
        }

        // データを受信した場合の処理
        if (serialPort != null && serialPort.IsOpen && serialPort.BytesToRead > 0)
        {
            string data = serialPort.ReadLine();
            Debug.Log("Received: " + data);
        }
    }

    void OnApplicationQuit()
    {
        Close();
    }

    public void Open()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
        serialPort.ReadTimeout = 1000;
        Debug.Log("Serial Port Opened");
    }

    public void Close()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial Port Closed");
        }
    }

    public void Write(string message)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine(message);
            serialPort.BaseStream.Flush();
        }
    }
}

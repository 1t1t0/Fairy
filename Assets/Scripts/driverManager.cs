using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class driverManager : MonoBehaviour
{
    public bool move=false;
    private int pulse; 
    SerialPort serialPort = new SerialPort("COM4", 115200); 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(move){
            if(Input.GetKey(KeyCode.RightArrow)){
                pulse=500;
                string data = pulse.ToString();
                serialPort.WriteLine(data);
            }else if(Input.GetKey(KeyCode.LeftArrow)){
                pulse=-500;
                string data = pulse.ToString();
                serialPort.WriteLine(data);
            }
        }
    }
}

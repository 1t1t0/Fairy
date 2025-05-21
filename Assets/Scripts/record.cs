using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class record : MonoBehaviour
{
    private StreamWriter sw;
    // Start is called before the first frame update
    void Start()
    {
        sw = new StreamWriter(@"SaveData2.csv", true, Encoding.GetEncoding("Shift_JIS"));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)){
            Transform myTransform=this.transform;

            Vector3 worldPos = myTransform.position;
            float x = worldPos.x;
            float y = worldPos.y;
            float z = worldPos.z;

            string[] s1  = {Convert.ToString(x), Convert.ToString(y), Convert.ToString(z)}; 
            string s2  = string.Join(",", s1);
            sw.WriteLine(s2);  
            Debug.Log("record");
        }

         if (Input.GetKeyDown(KeyCode.Return))
        {
            sw.Close();
            Debug.Log("finish");
        }
    }
}

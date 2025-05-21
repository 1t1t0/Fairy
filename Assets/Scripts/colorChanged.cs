using UnityEngine;
using System;

public class colorChanged : MonoBehaviour
{
    public float x_x, y_y;
    public float change;
    private float a, b;
    float px=0;
    public float c; 

    private Vector3 previousPosition;  // 前回の位置を保持

    void Start()
    {
        // 線形関数のパラメータ計算
        a = -0.09f;
        b = 0.06f;

        previousPosition = transform.position;  // 初期位置を設定
    }

    // zに応じてyの調整量を計算するメソッド
    float CalculateYAdjustment(float z)
    {
        return a * z + b;
    }

    // オブジェクトの位置を更新するメソッド
    public void UpdateP(float x, float y, float z)
    {
        float distanceMoved=x-px;
        px=x;

        //Debug.Log(distanceMoved);
        // z値に基づいてyの調整量を計算
        float yAdjustment = CalculateYAdjustment(z);

        // 新しい位置を計算
       // Vector3 newPosition = new Vector3(x_x * (x + 0.02f), y_y * (y - yAdjustment + 0.005f), 0.4f);
        Vector3 newPosition = new Vector3(x_x*x, 0, 0.4f);


        // 前回の位置との移動距離を計算
        //float distanceMoved = Vector3.Distance(previousPosition, newPosition);

        // 移動距離が5mmを超えた場合に赤色に、それ以下は白色に設定
        if (Math.Abs(distanceMoved) > change)  // 5mm = 0.005m
        {
            GetComponent<Renderer>().material.color = Color.red;  // 赤に変更
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;  // 白に変更
        }

        // 位置を更新
        transform.position = newPosition;

        // 前回の位置を更新
        previousPosition = newPosition;
    }

    // オブジェクトを中心にセットする関数
    public void SetPosition()
    {
        Vector3 newPosition = new Vector3(0.03f, -0.03f, 0.4f);  // 左右、上下の動きが反転している場合、符号を反転
        transform.position = newPosition;
    }
}

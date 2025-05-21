using UnityEngine;

public class getPoint : MonoBehaviour
{
    public float xx, yy;
    private float a, b;
    public float adjustQua;

    void Start()
    {
        // 線形関数のパラメータ計算
        a = -0.09f;
        b = 0.06f;
    }

    // zに応じてyの調整量を計算するメソッド
    float CalculateYAdjustment(float z)
    {
        return a * z + b;
    }

    // オブジェクトの位置を更新するメソッド
    public void UpdatePosition(float x, float y, float z)
    {  
        // yの調整量を計算
        float yAdjustment = CalculateYAdjustment(z);
        //Debug.Log(yAdjustment);

        // 受信した座標に基づいてSphereの位置を更新
        //Vector3 newPosition = new Vector3(xx * (x + 0.04f), yy * (y +0.04f), 0.4f);  // 左右、上下の動きが反転している場合、符号を反転
        Vector3 newPosition = new Vector3(xx * x , yy * y +0.25f , 0.4f);  // 左右、上下の動きが反転している場合、符号を反転
        
        transform.position = newPosition;
    }

    public void adjustPos(){
        //float hor = Input.GetAxis("Horizontal");
        //float ver = Input.GetAxis("Vertical");
        Vector3 tmp = GameObject.Find("pulse").transform.position;
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            GameObject.Find("pulse").transform.position = new Vector3(tmp.x + adjustQua, tmp.y, tmp.z);
        }else if(Input.GetKeyDown(KeyCode.RightArrow)){
            GameObject.Find("pulse").transform.position = new Vector3(tmp.x - adjustQua, tmp.y, tmp.z);
        }else if(Input.GetKeyDown(KeyCode.UpArrow)){
            GameObject.Find("pulse").transform.position = new Vector3(tmp.x, tmp.y + adjustQua, tmp.z);
        }else if(Input.GetKeyDown(KeyCode.DownArrow)){
            GameObject.Find("pulse").transform.position = new Vector3(tmp.x, tmp.y - adjustQua, tmp.z);
        }
    }

    //オブジェクトを中心にセットする関数
    public void setPosition(){
        Vector3 newPosition = new Vector3(0.03f, -0.03f, 0.4f);  // 左右、上下の動きが反転している場合、符号を反転
        transform.position = newPosition;
    }

}

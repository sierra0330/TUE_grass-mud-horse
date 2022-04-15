using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ball : MonoBehaviour
{
    //生成的位置
    public GameObject instantiatePos;
    //指定的球
    public GameObject ball;
    //抓取攝影機
    public GameObject camera;
    //抓取攝影機的名字
    public string cameraName;
    //球的位置
    GameObject ballClone;
    //在手上是與否
    static public bool inHands = false;


    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find(cameraName);
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void CreateBall()
    {
        
        if(!inHands){
            //手上沒球時，會生成出clone球
            ballClone = Instantiate(ball, instantiatePos.transform.position, instantiatePos.transform.rotation);
            //將clone球帶入到攝影機為母體裡面（套用攝影機的位置）
            ballClone.transform.SetParent(camera.transform);
            //將clone球的位置透過自身的位置做改變
            //ballClone.transform.localPosition = new Vector3(0f, -0.2f, 0.35f);

            ballClone.transform.localPosition = new Vector3(0f, -0.2f, 0.5f);
            ballClone.transform.localRotation = Quaternion.Euler(-27, 55, 0);

            inHands = true;
        }
        else{
            //將clone球銷毀
            Destroy(ballClone);
            inHands = false;
        }
        
    }

    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LlamaAction1 : MonoBehaviour
{
    
    public int State;
    public NavMeshAgent Llama;
    GameObject Llamaskin;

    // Camera
    GameObject Camera;

    // Llama's position
    public int StopDis = 5;
    public int WalkRadius = 10;
    Vector3 LlamaOriginPos;
    float LlamaRotationY;
    Quaternion LlamaRotation;

    // Canvas
    GameObject LlamaCanvasGroup;
    GameObject LlamaImages;
    public Sprite Pic1;
    public Sprite Pic2;

    // Llama's state judge
    bool LlamaFull = false;
    bool LlamaWool = false;
    bool Arrival = false;
    bool GrowWool = false;

    // Llama's wool state
    bool PressDown = false; 
    bool TowardLlama = false;
    float PressDownTime = 0.0f;
    public float HoldTime;

    // Llama's time
    float Timer1 = 0.0f;
    float Timer2 = 0.0f;

    // Llama's child component
    Transform[] LlamaChild;


    // Start is called before the first frame update
    void Start()
    {
        Camera = GameObject.Find("Player");
        LlamaOriginPos = Llama.transform.position;
        LlamaRotationY = Llama.transform.eulerAngles.y;
        LlamaRotation = Llama.transform.rotation;
        
        // 抓取Llama的子物件
        LlamaChild = Llama.GetComponentsInChildren<Transform>();
        for(int i = 0; i < LlamaChild.Length; i++)
        {
            //Debug.Log(i + " " + LlamaChild[i]);
            Llamaskin = LlamaChild[6].gameObject;
            LlamaCanvasGroup = LlamaChild[7].gameObject;
            LlamaImages = LlamaChild[8].gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(State == 0){
            // Llama餵食判斷
            if(!LlamaFull)
            {
                if(HoldAction.IsBarrel)
                {
                    HoldThing();
                }
                else
                {
                    ReturnOrigin();
                }
            }
            else
            { 
                if(!GrowWool)
                {
                    Timer1 += Time.deltaTime;
                    if(Timer1 >= 5.0f)
                    {
                        ReturnOrigin();
                        if(Timer1 >= 30.0f)
                        {
                            GrowWool = true;
                            Wool(true);
                        }
                    }
                    else
                    {
                        // Canvas
                        LlamaCanvas(true, 2);
                    }
                }
            }

            // Llama剪毛判斷
            if(LlamaWool)
            {
                if(HoldAction.IsScissers)
                {
                    HoldThing();
                }
                else
                {
                    ReturnOrigin();
                }
            }
            else
            {
                if(GrowWool)
                {
                    Timer1 += Time.deltaTime;
                    ReturnOrigin();
                
                    if(Timer1 >= 20.0f)
                    {
                        GrowWool = false;
                        LlamaFull = false;
                    }
                }
            }
        }

        else if(State == 1)
        {
            if(!LlamaFull)
            {
                if(HoldAction.IsBarrel)
                {
                    HoldThing();
                    Timer2 = 0.0f;
                }
                else
                {
                    Timer2 += Time.deltaTime;
                    if(Timer2 >= 0.3f)
                    {
                        LlamaWander(true);
                    }
                    else
                    {
                        LlamaWander(false);
                    }
                }
            }   
            else
            {
                if(!GrowWool)
                {
                    Timer1 += Time.deltaTime;
                    if(Timer1 >= 5.0f)
                    {
                        LlamaWander(true);
                        if(Timer1 >= 30.0f)
                        {
                            GrowWool = true;
                            Wool(true);
                        }
                    }
                    else
                    {
                        // Canvas
                        LlamaCanvas(true, 2);
                        LlamaWander(false);
                    }
                }
            }

            // Llama剪毛判斷
            if(LlamaWool)
            {
                if(HoldAction.IsScissers)
                {
                    HoldThing();
                    Timer2 = 0.0f;
                }
                else
                {
                    Timer2 += Time.deltaTime;
                    if(Timer2 >= 0.3f)
                    {
                        LlamaWander(true);
                    }
                    else
                    {
                        LlamaWander(false);
                    }
                }
            }
            else
            {
                if(GrowWool)
                {
                    Timer1 += Time.deltaTime;
                    if(Timer1 <= 0.3f)
                    {
                        LlamaWander(false);
                    }
                    else if(0.5f < Timer1 && Timer1 < 20.0f)
                    {
                        LlamaWander(true);
                    }
                    else if(Timer1 >= 20.0f)
                    {
                        LlamaWander(true);
                        GrowWool = false;
                        LlamaFull = false;
                    }
                    
                }
            }
        }
    }


    public void EatGrass()
    {
        if(Arrival && !LlamaFull && HoldAction.IsBarrel)
        {
            //Debug.Log("LlamaEat");

            Destroy(HoldAction.GrassClone);
            HoldAction.InHands = false;
            HoldAction.IsBarrel = false;

            Arrival = false;
            LlamaFull = true;
        }
    }


    public void HoldThing()
    {
        if(Vector3.Distance(Llama.transform.position, Camera.transform.position) >= StopDis)
        {
            LlamaAnimator(false);
            Llama.destination = Camera.transform.position;
            Arrival = false;
        }
        else
        {
            // 靠近player時調整視角，讓llama注視著player
            Vector3 LlamaLook = new Vector3(Camera.transform.position.x, 0.0f, Camera.transform.position.z) - new Vector3(Llama.transform.position.x, 0.0f, Llama.transform.position.z);
            Quaternion LookPlayer = Quaternion.LookRotation(LlamaLook);
            Llama.transform.rotation = Quaternion.Lerp(Llama.transform.rotation, LookPlayer, Time.deltaTime * 5);

            // Canvas
            LlamaCanvas(true, 1);
            // Llama會停下來，做等待的動作
            LlamaAnimator(true);
            Arrival = true;
            Timer1 = 0.0f;

            if(LlamaWool)
            {
                CutWool();
            }
        }
    }


    public void ReturnOrigin()
    {
        LlamaCanvas(false, 0);
        LlamaAnimator(false);
        Llama.destination = LlamaOriginPos;

        if(Vector3.Distance(Llama.transform.position, LlamaOriginPos) <= 1)
        {
            if(Llama.transform.eulerAngles.y != LlamaRotationY)
            {
                Llama.transform.rotation = Quaternion.Lerp(Llama.transform.rotation, LlamaRotation, Time.deltaTime * 5);
            }
            else
            {
                LlamaAnimator(true);
            }
        }
    }


    public Vector3 RandomNavMeshLocation()
    {
        LlamaAnimator(false);
        Vector3 FinPos = Vector3.zero;
        Vector3 RandomPos = Random.insideUnitSphere * WalkRadius;
        RandomPos += Llama.transform.position;
        // SamplePosition：判斷指定的隨機位置是否在NavMesh中的可行走範圍
        if(NavMesh.SamplePosition(RandomPos, out NavMeshHit Hit, WalkRadius, 1))
        {
            FinPos = Hit.position;
        }
        return FinPos;
    }

    public void LlamaWander(bool Wander)
    {
        if(Wander)
        {
            LlamaCanvas(false, 0);
            // 隨機找位置
            // remainingDistance：距離忠鈿剩餘的移動距離
            // stoppingDistance：停止在距目前位置的距離
            if(Llama.remainingDistance <= Llama.stoppingDistance)
            {
                Llama.SetDestination(RandomNavMeshLocation());
            }
        }
        else
        {
            Llama.SetDestination(Llama.transform.position);
        }
        
    }

    
    public void LlamaAnimator(bool Stop)
    {
        if(Stop)
        {
            Llama.isStopped = true;
            Llama.GetComponent<Animator>().SetBool("Move", false);
        }
        else
        {
            Llama.isStopped = false;
            Llama.GetComponent<Animator>().SetBool("Move", true);
        }
    }

    public void Wool(bool Wool)
    {
        if(Wool)
        {
            LlamaWool = true;
            Llamaskin.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
        }
        else
        {
            LlamaWool = false;
            Llamaskin.GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
        }
    }

    public void LlamaCanvas(bool Show, int Pic)
    {
        if(Show)
        {
            LlamaCanvasGroup.GetComponent<CanvasGroup>().alpha = 1;
            if(Pic == 1)
            {
                LlamaImages.GetComponent<Image>().sprite = Pic1;
            }
            else if(Pic == 2)
            {
                LlamaImages.GetComponent<Image>().sprite = Pic2;
            }
        }
        else
        {   
            LlamaCanvasGroup.GetComponent<CanvasGroup>().alpha = 0;
        }
    }



    public void PointLlama()
    {
        TowardLlama = true;
    }
    public void NotPointLlama()
    {
        TowardLlama = false;
    }

    // Llama's cut wool
    public void Press()
    {
        if(Arrival && LlamaWool && HoldAction.IsScissers)
        {
            PressDown = true;
        }
    }

    public void Release()
    {
        Reset();
    }

    private void Reset() 
    {
        PressDown = false;
        PressDownTime = 0.0f;
    }

    public void CutWool()
    {
        //Debug.Log(PressDown);
        if(PressDown && TowardLlama)
        {
            //Debug.Log(PressDownTime);
            PressDownTime += Time.deltaTime;
            if(PressDownTime >= HoldTime)
            {
                Arrival = false;
                Wool(false);
                Reset();
            }
        }
    }

}

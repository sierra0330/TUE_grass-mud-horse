using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LlamaAction : MonoBehaviour
{

    public int State;
    public NavMeshAgent Llama;
    public float StopDis;
    public GameObject LlamaSkin;
    public float WalkRadius;

    //canvas 圖片
    public GameObject LlamaCanvasGroup;
    public GameObject LlamaImages;
    public Sprite Pic1;
    public Sprite Pic2;


    GameObject Camera;

    // llama位置狀態
    Vector3 LlamaOriginPos;
    float LlamaRotationY;
    Quaternion LlamaRotation;


    bool LlamaFull = false;
    bool WantEat = false;

    float Timer1 = 0.0f;
    float Timer2 = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Camera = GameObject.Find("Player");
        LlamaOriginPos = Llama.transform.position;
        LlamaRotationY = Llama.transform.eulerAngles.y;
        LlamaRotation = Llama.transform.rotation;
        
    }

    // Update is called once per frame
    void Update()
    {
        //發呆的llama（State = 0）
        if(State == 0)
        {
            if(!LlamaFull)
            {
                if(HoldAction.InHands && HoldAction.IsBarrel)
                {
                    HoldGrass();
                }
                //返回原點
                else
                {   
                    LlamaCanvas(false, 0);
                    ReturnOrigin();
                }
            }
            else
            {
                Timer1 += Time.deltaTime;
                if(WantEat)
                {
                    //在原地吃5f時間，再回到原地
                    if(Timer1 >= 5.0f)
                    {
                        LlamaCanvas(false, 0);
                        ReturnOrigin();
                        //過30f時間，長毛
                        if(Timer1 >= 30f)
                        {
                            WantEat = false;
                        }
                    }
                    else
                    {
                        LlamaCanvas(true, 2);
                    }
                }
                else
                {
                    //長毛
                    LlamaSkin.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
                }
            }
        }

        //走路的llama（State == 1）
        else if (State == 1)
        {
            if(!LlamaFull)
            {
                if(HoldAction.InHands && HoldAction.IsBarrel)
                {
                    HoldGrass();
                    Timer2 = 0.0f;

                }
                else
                {
                    LlamaCanvas(false, 0);

                    //判斷走路的llama中途將原本的目的地（攝影機的位置）中斷，變成現在所在的位置
                    //所以每當拿起草，計時器就會刷新，放回草時，開始計時
                    //目的就是將原本 要到達攝影機的位置，才開始隨便走的情況中斷
                    //直接就是中途直接換成隨便走的情況
                    Timer2 += Time.deltaTime;
                    if(Timer2 >= 0.5f)
                    {
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
            }
            else
            {
                Timer1 += Time.deltaTime;
                if(WantEat)
                {
                    if(Timer1 >= 5f)
                    {
                        LlamaCanvas(false, 0);
                        
                        //隨機找位置
                        //remainingdistance：距離終點剩餘的移動距離
                        //stoppingdistance：停止在距目前位置的距離
                        if(Llama.remainingDistance <= Llama.stoppingDistance)
                        {
                            Llama.SetDestination(RandomNavMeshLocation());
                        }
                        if(Timer1 >= 30f)
                        {
                            WantEat = false;
                        }
                    }
                    else
                    {
                        LlamaCanvas(true, 2);
                        Llama.SetDestination(Llama.transform.position);
                    }
                }
                else
                {
                    //隨機找位置
                    if(Llama.remainingDistance <= Llama.stoppingDistance)
                    {
                        Llama.SetDestination(RandomNavMeshLocation());
                    }
                    LlamaSkin.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
                }
            }
        }

    }


    public void EatGrass()
    {
        if(HoldAction.InHands && HoldAction.IsBarrel && WantEat)
        {
            if(!LlamaFull)
            {
                Destroy(HoldAction.GrassClone);
                HoldAction.InHands = false;
                HoldAction.IsBarrel = false;
            }
            LlamaFull = true;
            
            
        }
    }


    public void HoldGrass()
    {
        if(Vector3.Distance(Llama.transform.position, Camera.transform.position) >= StopDis)
        {
            LlamaAnimator(false);
            Llama.destination = Camera.transform.position;
            WantEat = false;
        }
        else
        {
            //走路的llama走進StopDis的範圍內時，導正面向player
            if(State == 1)
            {
                Vector3 LlamaLook = new Vector3(Camera.transform.position.x, 0f, Camera.transform.position.z) - new Vector3(Llama.transform.position.x, 0f, Llama.transform.position.z);
                Quaternion LookPlayer = Quaternion.LookRotation(LlamaLook);
                Llama.transform.rotation = Quaternion.Lerp(Llama.transform.rotation, LookPlayer, Time.deltaTime * 5);
            }

            //llama會停下來，動畫變成等待餵食
            //canvas的顯示與否
            LlamaCanvas(true, 1);

            LlamaAnimator(true);
            WantEat = true;
            Timer1 = 0f;

        }
    }


    public void ReturnOrigin()
    {
        LlamaAnimator(false);
        Llama.destination = LlamaOriginPos;

        //轉到原本的轉向
        if(Vector3.Distance(Llama.transform.position, LlamaOriginPos) <= 1)
        {
            if(Llama.transform.eulerAngles.y != LlamaRotationY)
            {   
                Llama.transform.rotation = Quaternion.Lerp(Llama.transform.rotation, LlamaRotation, Time.deltaTime * 15);
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
        //SamplePosition：判斷指定的隨機位置是否在NavMesh中的可行走範圍
        if(NavMesh.SamplePosition(RandomPos, out NavMeshHit Hit, WalkRadius, 1))
        {
            FinPos = Hit.position;
        }
        return FinPos;
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

}

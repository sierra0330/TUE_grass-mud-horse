using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LlamaEat : MonoBehaviour
{
    public NavMeshAgent llama;
    public GameObject camera;
    public string cameraName;
    public float stopDis;

    bool llamaFinPos = false;
    bool llamaGross = false;
    Vector3 llamaPos;

    

    public GameObject canvasGroup;
    public GameObject images;
    public Sprite image2;


    // Start is called before the first frame update
    void Start()
    {
        llamaPos = llama.transform.position;
        camera = GameObject.Find(cameraName);
    }

    // Update is called once per frame
    void Update()
    {
        if(!llamaFinPos)
        {
            if(Ball.inHands)
            {
                if(Vector3.Distance(llama.transform.position, camera.transform.position) > stopDis)
                {
                    llama.destination = camera.transform.position;
                }
                else
                {
                    canvasGroup.GetComponent<CanvasGroup>().alpha = 1;
                    llama.isStopped = true;
                    llamaFinPos = true;
                }
            }
            else
            {
                llama.destination = llamaPos;
            }   
        }
        else
        {
            if(llamaGross)
            {
                images.GetComponent<Image>().sprite = image2;
                llama.GetComponent<MeshRenderer>().material.color = Color.green;
                //開啟導航
                llama.isStopped = false;
                llama.destination = llamaPos;
            }
            else
            {
                llama.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
    }

    public void FoodLlama()
    {
        if(llamaFinPos){
            if(Ball.inHands)
            {
                llamaGross = true;
            }
            
        }
    }


}

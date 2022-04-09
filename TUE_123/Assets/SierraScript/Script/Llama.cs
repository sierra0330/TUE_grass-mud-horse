using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Llama : MonoBehaviour
{
    public NavMeshAgent llama;
    public float walkRadius;

    // Start is called before the first frame update
    void Start()
    {
        llama = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        //判斷是否已經到了指定的隨機地點
        //remainingdistance：距離終點剩餘的移動距離
        //stoppingdistance：停止在距目前位置的距離
        if(llama != null && llama.remainingDistance <= llama.stoppingDistance)
        {
            llama.SetDestination(RandomNavMeshLocation());
        }
    }

    public Vector3 RandomNavMeshLocation()
    {
        Vector3 finalPos = Vector3.zero;
        Vector3 randomPos = Random.insideUnitSphere * walkRadius;
        randomPos += transform.position;
        //sampleposition：判斷指定的隨機地點是否在NavMesh中的可行走範圍
        if(NavMesh.SamplePosition(randomPos, out NavMeshHit hit, walkRadius, 1))
        {
            finalPos = hit.position;
        }
        return finalPos;
    }
}

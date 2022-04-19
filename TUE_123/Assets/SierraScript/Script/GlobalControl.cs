using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour
{

    public static GlobalControl Instance;
    void Awake()
    {

        // 當引用（instance）為空時，讓此gameobject不要刪除
        // 當引用（instance）不為空時，刪除此gameobject
        // 以免回到初始場景時，又在創造出一個
        if(Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);  
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

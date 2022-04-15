using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour
{

    public static GlobalControl Instance;

    public bool llamaWool = false;

    void Awake()
    {
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
        Debug.Log(llamaWool);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

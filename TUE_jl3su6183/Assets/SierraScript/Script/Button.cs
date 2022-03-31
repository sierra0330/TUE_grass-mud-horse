using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene0()
    {
        SceneManager.LoadScene(0);
    }

    public void ChangeScene1()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        
    }
}

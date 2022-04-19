using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldAction : MonoBehaviour
{
    public static bool InHands = false;

    /* 草 */
    [Header("羊駝食物")]
    public GameObject Grass;
    [Header("食物出生位置")]
    public GameObject Barrel;
    public static GameObject GrassClone;
    
    GameObject Camera;

    [Header("剪刀")]
    public GameObject Scissors;
    Vector3 ScissorsPos;

    public static bool IsBarrel = false;
    public static bool IsScissers = false;



    // Start is called before the first frame update
    void Start()
    {
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        ScissorsPos = Scissors.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void HoldGrass()
    {
        if(!InHands && !IsBarrel)
        {
            //當手中沒東西時，創造一把草位於木箱的位置上
            GrassClone = Instantiate(Grass, Barrel.transform.position, Barrel.transform.rotation);
            //將clone出來的草帶入到主要攝影機(母體中)，可以隨著攝影機的移動也跟著移動
            GrassClone.transform.SetParent(Camera.transform);
            //根據攝影機當下的位置做更動
            GrassClone.transform.localPosition = new Vector3(0f, -0.2f, 0.5f);
            GrassClone.transform.localRotation = Quaternion.Euler(-27, 55, 0);

            IsBarrel = true;
            InHands = true;

        }
        else if(InHands && IsBarrel && GrassClone != null)
        {
            //摧毀草
            Destroy(GrassClone);
            IsBarrel = false;
            InHands = false;
        }
    }

    
    public void HoldScissors()
    {
        if(!InHands && !IsScissers)
        {
            Scissors.transform.SetParent(Camera.transform);
            Scissors.transform.localPosition = new Vector3(0f, -0.5f, 1f);
            IsScissers = true;
            InHands = true;
        }
        else if(InHands && IsScissers && Scissors != null)
        {
            Scissors.transform.SetParent(null);
            Scissors.transform.position = ScissorsPos;

            IsScissers = false;
            InHands = false;
        }
    }
    
}

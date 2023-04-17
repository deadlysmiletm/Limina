using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChanger : MonoBehaviour
{
    public GameObject Player;
    public GameObject[] Luces = new GameObject[2];
    public bool changer = false;

    void Update()
    {
        Light();
    }

    public void Light()
    {
        if (changer == true)
        {
            if (Luces[0].active == true)
            {
                Luces[0].SetActive(false);
                Luces[1].SetActive(true);
            }
            else if (Luces[1].active == true)
            {
                Luces[1].SetActive(false);
                Luces[0].SetActive(true);
            }
            changer = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject == Player)
        {
            changer = true;
        }
    }

}

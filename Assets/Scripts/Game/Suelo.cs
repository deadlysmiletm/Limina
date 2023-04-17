using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suelo : MonoBehaviour {
    
    //Separo las variables
    public bool Caer = false;
    private bool CuentaAtras;
    public float Timer;

    //De los componenetes
    private Rigidbody2D rb;
    public GameObject Soporte;
    private BoxCollider2D bxSoporte;
    private BoxCollider2D bx;

	void Start ()
    {
		if(Caer == true)
        {
            bxSoporte = Soporte.GetComponent<BoxCollider2D>();
            bx = this.GetComponent<BoxCollider2D>();
        }
	}
	
	void Update ()
    {
        //Si el suelo va a ser una plataforma que se cae, aquí se realiza la cuenta atrás.
		if(CuentaAtras == true)
        {
            Timer -= Time.deltaTime;
            if(Timer <= 0)
            {
                bxSoporte.enabled = false;
                bx.enabled = false;
            }
        }
	}

    //Función que detecta colisiones y activa la cuenta regresiva si es el player.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject Player = GameObject.Find("Player");
        if (Caer == true && collision.gameObject == Player)
        {
            CuentaAtras = true; 
        }
        collision.gameObject.SendMessage("OnFloor", SendMessageOptions.DontRequireReceiver);
        collision.gameObject.SendMessage("OnShoot", SendMessageOptions.DontRequireReceiver);
    }


}

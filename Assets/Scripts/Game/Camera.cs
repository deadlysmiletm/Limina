using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camera : MonoBehaviour {

    //Ordeno todos los componentes juntos.
    private BoxCollider2D bc;
    public GameObject Player;
    private PlayerBrain pb;
    private BoxCollider2D Playerbx;
    private BoxCollider2D Exitbx;
    private GameObject Exit;
    public Text Pause;
    public Text GameOver;
    public Text Victory;
    public Text Teleport;

    //Ordeno todas las variables que no son componentes juntas.
    public bool seguimiento = false;
    public string nivelActual;
    public float LímiteIzq;
    public bool Pausa = false;
    public float TiempoMensaje = 6;


    void Start ()
    {
        Player = GameObject.Find("Player");
        pb = Player.GetComponent<PlayerBrain>();
        Playerbx = Player.GetComponent<BoxCollider2D>();
        bc = this.gameObject.GetComponent<BoxCollider2D>();
        bc.enabled = true;

        Exit = GameObject.Find("Exit");
        Exitbx = Exit.GetComponent<BoxCollider2D>();

        Pause.gameObject.SetActive(false);
        Victory.gameObject.SetActive(false);
        GameOver.gameObject.SetActive(false);
        Teleport.gameObject.SetActive(false);
    }

    void Update ()
    {
        if (seguimiento == true)
        {
            //Función encargada de que la cámara siga al personaje.
            SeguimientoCámara();
        }

        //Aquí se muestra la excepción si el personaje muere.
        if(pb.alive == false)
        {
            seguimiento = false;
            GameOver.gameObject.SetActive(true);
        }

        //Aquí se muestra cuando el personaje llega a la meta del nivel.
        if(Playerbx.IsTouching(Exitbx) == true)
        {
            seguimiento = false;
            pb.Exit = true;
            Victory.gameObject.SetActive(true);
        }

        //Aquí se administra el mensaje al encontrar el activador del Teleport.
        if(pb.ActiveTeleport == true)
        {
            if (TiempoMensaje > 0)
            {
                Teleport.gameObject.SetActive(true);
                TiempoMensaje -= Time.deltaTime;
            }
            else
            {
                Teleport.gameObject.SetActive(false);
            }
            
        }

        //Poner pausa.
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Pausa == true)
            {
                Pausa = false;
            }
            else
            {
                Pausa = true;
            }
            MenúPausa();
        }
	}

    //Función encargada de determinar qué sucede en la pausa.
    public void MenúPausa()
    {
        if (Pausa == true)
        {
            Time.timeScale = 0;
            Pause.gameObject.SetActive(true);
        }
        else if (Pausa == false)
        {
            Time.timeScale = 1;
            Pause.gameObject.SetActive(false);
        }
    }

    //Aquí se inicia el seguimiento por parte de la cámara al jugador.
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.layer == 10)
        {
            seguimiento = true;
            bc.enabled = false;
        }
    }

    public void SeguimientoCámara()
    {
        if (Player.transform.position.x <= LímiteIzq)
        {
            seguimiento = false;
            bc.enabled = true;
        }
        else
        {
            this.transform.position = Player.transform.position + new Vector3(0, 7, -10);
        }
    }
}

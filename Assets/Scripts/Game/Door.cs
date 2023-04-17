using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    //Separo los componentes.
    public Vector3 Destino;
    public GameObject Player;
    public EdgeCollider2D bxChild;
    private BoxCollider2D bx;
    private SpriteRenderer sr;
    public Sprite Cerrada;
    public Sprite Abierta;
    public bool openSwitch;
    private AudioSource As;

    //De las variables booleanas.
    public bool Portal = false;
    public bool Bloqueada = false;

	void Start ()
    {
        As = this.GetComponent<AudioSource>();
        bx = this.gameObject.GetComponent<BoxCollider2D>();
        sr = this.gameObject.GetComponent<SpriteRenderer>();
        bxChild = this.gameObject.GetComponentInChildren<EdgeCollider2D>();

	}

    void Update()
    {
        if (openSwitch == true)
        {
            if (Input.GetButtonDown("Accion"))
            {
                Abrir();
            }
        }
    }

    //Función encargada de abrir la puerta.
    public void OnTriggerEnter2D(Collider2D collider)
    {
        //Player = collider.gameObject;

        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            openSwitch = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            openSwitch = false;
        }
    }

    private void Abrir()
    {
        As.Play();
        bx.enabled = false;
        bxChild.enabled = false;
        sr.sprite = Abierta;
    }
}

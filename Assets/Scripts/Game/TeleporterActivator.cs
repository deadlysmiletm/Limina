using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterActivator : MonoBehaviour {

    public GameObject Player;

	// Use this for initialization
	void Start ()
    {
        Player = GameObject.Find("Player");
	}

    //Al colisionar con el objeto el jugador desbloquea la posibilidad de usar Teleport
    //y luego destruye este objeto.
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject == Player)
        {
            PlayerBrain Teleport = Player.GetComponent<PlayerBrain>();
            Teleport.ActiveTeleport = true;
            Destroy(this.gameObject);
        }
    }
}

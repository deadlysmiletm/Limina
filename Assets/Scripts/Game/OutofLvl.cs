using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutofLvl : MonoBehaviour {

    //Función que envía un mensaje al player para asesinarlo por irse fuera del mapa.
    public void OnTriggerEnter2D(Collider2D collider)
    {
        collider.SendMessage("YouAreDead", SendMessageOptions.DontRequireReceiver);
    }
}

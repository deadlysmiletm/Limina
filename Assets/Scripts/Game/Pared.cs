using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pared : MonoBehaviour {

    //Función que envía mensajes para deterctar colisiones.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.SendMessage("OnWall", SendMessageOptions.DontRequireReceiver);
        collision.gameObject.SendMessage("OnEnemy", SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        collider.gameObject.SendMessage("OnShoot", SendMessageOptions.DontRequireReceiver);
    }
}

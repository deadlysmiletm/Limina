using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitEnemy : MonoBehaviour {

    //Límite para hacer que los enemigos switcheen en zonas que no lo harían normalmente, por ejemplo frente a una puerta.
    public void OnCollisionEnter2D(Collision2D collider)
    {
        collider.gameObject.SendMessage("OnEnemy", SendMessageOptions.DontRequireReceiver);
    }
}

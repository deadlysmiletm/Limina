using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour

{

    private AudioSource As;

    void Start()
    {
        As = this.GetComponent<AudioSource>();
    }

    //Función que envía un mensaje cuyo nombre es una función en el script del Player. La idea aquí es curarlo.
	private void OnTriggerEnter2D(Collider2D collider)
    {
        collider.SendMessage("HealthYourself", SendMessageOptions.DontRequireReceiver);
        As.Play();
    }
}

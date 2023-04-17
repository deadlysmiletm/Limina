using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Rigidbody2D rb;
    public float velocidad;
    public int dir = 1;
    public float DeathTime;

	void Start ()
    {
        //La bala es impulsada mediante física.
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector3.right * velocidad * dir * rb.mass, ForceMode2D.Impulse);
    }

    void Update()
    {
        //La bala se desytruye cuando su "DeathTime" llega a 0.
        Destroy(this.gameObject, DeathTime);
    }

    //Esta función indica si colisiona con un objeto o enemigo para que se destruya.
    public void OnShoot()
    {
        Destroy(this.gameObject);
    }
    
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.layer == LayerMask.NameToLayer("Puerta"))
        {
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class PlayerBrain : MonoBehaviour {

    //Separamos las variables que no son componentes.
    public float vida = 1;
    private float ValorAxis;
    public float velocidad;
    public float fuerzaSalto;
    public bool dirDer = true;
    public bool salto = false;
    private bool mov = true;
    private bool suelo = true;
    private bool correr = false;
    private float velCorrer;
    private float velNormal;
    private bool apuntar = false;
    private bool disparar = false;
    public bool Teletransportación = false;
    public bool ActiveTeleport;
    public bool alive = true;
    public bool hit = false;
    public bool Exit = false;

    //Todos los componentes van aquí.
    public UnityArmatureComponent Armature;
    public DragonBones.AnimationState animación;
    public GameObject Mira;
    public Vector3 MiraDer;
    public Vector3 MiraIzq;
    private Vector3 movLateral;
    public GameObject BulletPrefab;
    public Bullet BulletScript;
    public GameObject TeleportPoint;
    private SpriteRenderer srTeleport;
    private BoxCollider2D bxTeleport;
    public Color colorNormal;
    public Color Red;
    private bool TeleportNo = false;
    public Sprite Teleport;
    public Sprite TeleportVol;
    private Rigidbody2D rb;
    private BoxCollider2D bx;
    public AudioSource As;
    public AudioSource AsFx;

    public AudioClip LatidoRápido;
    public AudioClip LatidoMuyRápido;
    public AudioClip[] AccionesFx = new AudioClip[7];

	void Start ()
    {
        //Este primer bloque es el pilar principal de las animaciones.
        Armature = this.gameObject.GetComponent<UnityArmatureComponent>();
        animación = Armature._armature.animation.Play("Idle");
        Armature.AddEventListener(EventObject.LOOP_COMPLETE, Jump);
        Armature.AddEventListener(EventObject.LOOP_COMPLETE, Fire);
        Armature.AddEventListener(EventObject.LOOP_COMPLETE, TeleportMode);
        Armature.AddEventListener(EventObject.LOOP_COMPLETE, Life);

        rb = this.gameObject.GetComponent<Rigidbody2D>();
        bx = this.gameObject.GetComponent<BoxCollider2D>();
        srTeleport = TeleportPoint.GetComponent<SpriteRenderer>();
        bxTeleport = TeleportPoint.GetComponent<BoxCollider2D>();
        BulletScript = BulletPrefab.GetComponent<Bullet>();

        srTeleport.color = colorNormal;
        srTeleport.sprite = null;
        bxTeleport.enabled = false;

        velNormal = velocidad;
        velCorrer = velocidad * 2;
	}

    void Update()
    {
        if (Exit == false)
        {
            if (alive == true)
            {
                //Movimiento del personaje, de forma lateral.
                ValorAxis = Input.GetAxis("Horizontal");
                movLateral = Vector3.right * velocidad * ValorAxis;
                movLateral.y = rb.velocity.y;

                //Aquí se decide la dirección del personaje.
                if (mov == true)
                {
                    if (ValorAxis > 0)
                    {
                        dirDer = true;
                    }
                    else if (ValorAxis < 0)
                    {
                        dirDer = false;
                    }

                    rb.velocity = movLateral;
                }

                //Aquí se confirma si se puede usar Teleport.
                if (ActiveTeleport == true)
                {
                    TeleportControl();
                }

                JumpControl();

                if (salto == false && Teletransportación == false)
                {
                    AnimControlerMove();
                }
                if(animación.name == "Run" || animación.name == "Run_volteado")
                {
                    animación.timeScale = 1.40f;
                }
                else
                {
                    animación.timeScale = 1;
                }
                LifeSound();
                Muerte();
            }
        }
        else
        {
            rb.velocity = Vector3.right * velNormal;
            AnimMachine("Walk", 0.01f);
            Destroy(this.gameObject, 10);
        }

        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -velCorrer, velCorrer), Mathf.Clamp(rb.velocity.y, -velCorrer-3, velCorrer+3));
        if (AsFx.clip == AccionesFx[6])
        {
            AsFx.loop = false;
            AsFx.pitch = 1;
            AsFx.Play();
        }
    }

    //Función encargada de administrar el daño.
    public void Damage()
    {
        hit = true;
        vida -= 0.50f;
        //mov = true;
        Teletransportación = false;
        apuntar = false;
        disparar = false;
        srTeleport.sprite = null;
        bxTeleport.enabled = false;
        if (dirDer == true)
        {
            AnimMachine("Hit", 0.01f, 1);
            AsFx.clip = AccionesFx[1];
            AsFx.Play();
            AsFx.loop = false;
        }
        else
        {
            AnimMachine("Hit_volteado", 0.01f, 1);
            AsFx.clip = AccionesFx[1];
            AsFx.Play();
            AsFx.loop = false;
        }
    }

    //Función de recepción de mensaje cuando se está tocando el suelo.
    public void OnFloor()
    {
        if (salto == true)
        {
            AsFx.clip = AccionesFx[5];
            AsFx.Play();
        }

        suelo = true;
        mov = true;
    }

    //Función preparada para administrar las animaciones de forma más eficiente.
    public void AnimMachine(string animName, float FadeIn = 0.1f, int Veces = -1, float timeScale = 1)
    {
        if (animación.animationData.name == animName)
        { }
        else
        {
            Armature._armature.animation.timeScale = timeScale;
            animación = Armature._armature.animation.FadeIn(animName, FadeIn, Veces, 0);
        }
    }

    //Función que administra el movimiento lateral, el disparo y el correr.
    public void AnimControlerMove()
    {
        if (disparar == true)
        {
            if (Input.GetButtonDown("Fire"))
            {
                if (dirDer)
                {
                    Mira.transform.localPosition = MiraDer;
                    AnimMachine("Disparar", 0.01f, 1);
                    AsFx.clip = AccionesFx[2];
                    AsFx.loop = false;
                    AsFx.Play();
                    BulletScript.dir = 1;
                    AsFx.pitch = 1;
                }
                else
                {
                    Mira.transform.localPosition = MiraIzq;
                    AnimMachine("Disparar_volteado", 0.01f, 1);
                    AsFx.clip = AccionesFx[2];
                    AsFx.loop = false;
                    AsFx.Play();
                    BulletScript.dir = -1;

                }
                GameObject Bala = GameObject.Instantiate(BulletPrefab);
                Bala.transform.localPosition = Mira.transform.position;
                disparar = false;

            }
        }

        if (apuntar == false)
        {
            if (Input.GetButtonDown("Fire"))
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                if (dirDer == true)
                {
                    AnimMachine("Apuntar", 0.1f, 1, timeScale: 2);
                }
                else
                {
                    AnimMachine("Apuntar_volteado", 0.1f, 1, timeScale: 2);
                }
                apuntar = true;
            }
        }

        if (apuntar == true)
        {
            mov = false;
            if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Run") || Input.GetButtonDown("Jump") || Input.GetButtonDown("Teleport"))
            {
                if (dirDer == true)
                {
                    AnimMachine("Desapuntar", 0.01f, 1, timeScale: 2);
                }
                else
                {
                    AnimMachine("Desapuntar_volteado", 0.01f, 1, timeScale: 2);
                }
            }
        }

        if (Input.GetButton("Run"))
        {
            correr = true;
        }
        else
        {
            correr = false;
        }
        if (ValorAxis > 0 && mov == true && suelo == true)
        {
            if (correr == true)
            {
                AnimMachine("Run", 0.01f);
                velocidad = velCorrer;
                AsFx.loop = true;
                AsFx.clip = AccionesFx[0];
                AsFx.pitch = 1;
                if (AsFx.isPlaying == false)
                {
                    AsFx.Play();
                }
            }
            else
            {
                AnimMachine("Walk", 0.01f);
                velocidad = velNormal;
                AsFx.loop = true;
                AsFx.clip = AccionesFx[0];
                AsFx.pitch = 0.74f;
                if (AsFx.isPlaying == false)
                {
                    AsFx.Play();
                }
            }
            apuntar = false;
            disparar = false;
            hit = false;
        }
        else if (ValorAxis < 0 && mov == true && suelo == true)
        {
            if (correr == false)
            {
                AnimMachine("Walk_volteado", 0.01f);
                velocidad = velNormal;
                AsFx.loop = true;
                AsFx.clip = AccionesFx[0];
                AsFx.pitch = 0.74f;
                if (AsFx.isPlaying == false)
                {
                    AsFx.Play();
                }
            }
            else
            {
                AnimMachine("Run_volteado", 0.01f);
                velocidad = velCorrer;
                AsFx.loop = true;
                AsFx.clip = AccionesFx[0];
                AsFx.pitch = 1;
                if (AsFx.isPlaying == false)
                {
                    AsFx.Play();
                }
            }
            apuntar = false;
            disparar = false;
        }

        else if (ValorAxis == 0 && apuntar == false && hit == false)
        {
            if (dirDer == true)
            {
                AnimMachine("Idle");
                AsFx.Stop();
            }
            else
            {
                AnimMachine("Idle_volteado");
                AsFx.Stop();
            }
        }
    }

    //Función especial donde se administra las condiciones durante la animación de salto.
    //Por ejemplo: que tras la previa de salto se le añada impulso al rigidbody.
    public void Jump(string type, EventObject eventObject)
    {
        switch (type)
        {
            case EventObject.LOOP_COMPLETE:
                if (dirDer == true)
                {
                    if (eventObject.animationState.name == "Salto_Inicio")
                    {
                        mov = true;
                        rb.AddForce(Vector2.up * fuerzaSalto * rb.mass, ForceMode2D.Impulse);
                        AnimMachine("Salto_Aire", 0.01f, 1); AsFx.clip = AccionesFx[1];
                        AsFx.clip = AccionesFx[4];
                        AsFx.pitch = 1;
                        AsFx.Play();
                        AsFx.loop = false;
                    }
                    else if (eventObject.animationState.name == "Salto_Final")
                    {
                        salto = false;
                    }
                }
                else
                {
                    if (eventObject.animationState.name == "Salto_volteado_Inicio")
                    {
                        rb.AddForce(Vector2.up * fuerzaSalto * rb.mass, ForceMode2D.Impulse);
                        AnimMachine("Salto_volteado_Aire", 0.01f, 1);
                        AsFx.clip = AccionesFx[4];
                        AsFx.pitch = 1;
                        AsFx.Play();
                        mov = true;
                        AsFx.loop = false;
                    }
                    else if (eventObject.animationState.name == "Salto_volteado_Final")
                    {
                        salto = false;
                    }
                }
                break;
                

        }
    }

    //Función que maneja el control de salto, así como también la transición de salto a caída o la caída únicamente.
    private void JumpControl()
    {
        if (Input.GetButton("Jump") && salto == false && apuntar == false && Teletransportación == false)
        {
            mov = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (dirDer == true)
            {
                AnimMachine("Salto_Inicio", 0.1f, 1, timeScale: 2);
            }
            else
            {
                AnimMachine("Salto_volteado_Inicio", 0.1f, 1, timeScale: 2);
            }
            salto = true;
            suelo = false;
        }

        if (rb.velocity.y < -0.5)
        {
            if (AsFx.clip == AccionesFx[6])
            {
                AsFx.loop = false;
                AsFx.Play();
            }
            else
            {
                AsFx.Stop();
            }
            salto = true;
            suelo = false;
            if (dirDer == true)
            {
                AnimMachine("Salto_Tope", 0.01f, 1);
                rb.AddForce(new Vector2(2, 0) * rb.mass, ForceMode2D.Impulse);
                

            }
            else
            {
                AnimMachine("Salto_volteado_Tope", 0.01f, 1);
                rb.AddForce(new Vector2(-2, 0) * rb.mass, ForceMode2D.Impulse);
            }
        }

        if (salto == true && suelo == true)
        {
            if (dirDer == true)
            {
                AnimMachine("Salto_Final", 0.01f, 1);
            }
            else
            {
                AnimMachine("Salto_volteado_Final", 0.01f, 1);
            }
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    //Función especial de condiciones para la acción de apuntar y disparar.
    public void Fire(string type, EventObject eventObject)
    {
        switch(type)
        {
            case EventObject.LOOP_COMPLETE:
                if (dirDer == true)
                {
                    if (eventObject.animationState.name == "Apuntar")
                    {
                        disparar = true;
                    }
                    else if (eventObject.animationState.name == "Disparar")
                    {
                        AnimMachine("DisparoStand", 0.01f, 1, timeScale: 2.5f);
                        disparar = true;
                    }
                }
                else
                {
                    if (eventObject.animationState.name == "Apuntar_volteado")
                    {
                        disparar = true;
                    }
                    else if (eventObject.animationState.name == "Disparar_volteado")
                    {
                        AnimMachine("DisparoStand_volteado", 0.01f, 1, timeScale: 2.5f);
                        disparar = true;
                    }
                }
                if (eventObject.animationState.name == "Desapuntar" || eventObject.animationState.name == "Desapuntar_volteado")
                {
                    disparar = false;
                    apuntar = false;
                    mov = true;
                }
                break;
        }
    }

    //Función encargada de administrar el control del Teleport.
    private void TeleportControl()
    {
        if (Teletransportación == true)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            TeleportPoint.transform.position += movLateral * Time.deltaTime;

            if (Input.GetButtonDown("Fire") || Input.GetButtonDown("Jump"))
            {
                srTeleport.sprite = null;
                bxTeleport.enabled = false;
                TeleportPoint.transform.position = this.transform.position;
                if (dirDer == true)
                {
                    AnimMachine("Teleport_cancel", 0.01f, 1);
                }
                else
                {
                    AnimMachine("Teleport_volteado_cancel", 0.01f, 1);
                }
            }
        }

        if (Input.GetButtonDown("Teleport") && salto == false && apuntar == false)
        {
            if (Teletransportación == false)
            {
                AsFx.Stop();
                mov = false;
                Teletransportación = true;
                TeleportPoint.transform.position = this.transform.position + new Vector3(0, 4.4f, 0);
                srTeleport.color = colorNormal;
                if (dirDer == true)
                {
                    AnimMachine("Teleport", 0.01f, 1);
                }
                else
                {
                    AnimMachine("Teleport_volteado", 0.01f, 1);
                }
            }
            else
            {
                if (hit == true)
                {
                    srTeleport.sprite = null;
                    mov = true;
                    Teletransportación = false;
                }
                else if (TeleportNo == false)
                {
                    this.transform.position = TeleportPoint.transform.position + new Vector3(0, 1, 0);
                    srTeleport.sprite = null;
                    bxTeleport.enabled = false;
                    mov = true;
                    Teletransportación = false;
                    AsFx.clip = AccionesFx[6];
                }
            }
        }
    }

    //Función especial de condiciones del Teleport.
    public void TeleportMode(string type, EventObject eventObject)
    {
        switch (type)
        {
            case EventObject.LOOP_COMPLETE:
                if (eventObject.animationState.name == "Teleport" || eventObject.animationState.name == "Teleport_volteado")
                {
                    if (dirDer == true)
                    {
                        srTeleport.sprite = Teleport;
                    }
                    else
                    {
                        srTeleport.sprite = TeleportVol;
                    }
                    bxTeleport.enabled = true;
                }
                else if(eventObject.animationState.name == "Teleport_cancel" || eventObject.animationState.name == "Teleport_volteado_cancel")
                {
                    srTeleport.sprite = null;
                    mov = true;
                    Teletransportación = false;
                }

                break;
        }
        
    }

    //Función especial de condiciones para vida.
    public void Life(string type, EventObject eventObject)
    {
        switch (type)
        {
            case EventObject.LOOP_COMPLETE:
                if (eventObject.animationState.name == "Hit" || eventObject.animationState.name == "Hit_volteado")
                {
                    hit = false;
                    mov = true;
                    Muerte();
                }
                break;
        }
    }

    //Función donde se reconoce el daño recibido.
    public void OnTriggerEnter2D(Collider2D daño)
    {
        if(daño.gameObject.layer == LayerMask.NameToLayer("EnemyAttack"))
        {
            Damage();
        }            
    }
    
    //Función donde en Teleport se detecta las colisiones y evita que viajemos a ese lugar.
    public void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.gameObject)
        {
            srTeleport.color = Red;
            TeleportNo = true;
        }
        else
        {
            TeleportNo = false;
        }
    }

    //Función donde se indica donde sí podemos usar Teleport.
    public void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject)
        {
            srTeleport.color = colorNormal;
            TeleportNo = false;
        }
    }

    //Función de recepción de mensaje del GameObject que indica cuando caemos al vacío.
    public void YouAreDead()
    {
        vida = 0;
        alive = false;
    }

    //Función encargado de curar al personaje.
    public void HealthYourself()
    {
        vida = 1;
    }

    //Función que administra el sonido de latidos dependiendo de la vida.
    private void LifeSound()
    {
        if (vida == 1)
        {
            As.clip = null;
        }
        else if (vida == 0.50f)
        {
            As.clip = LatidoRápido;
        }
        else if (vida == 0.25f)
        {
            As.clip = LatidoMuyRápido;
        }
        if (As.isPlaying == false)
        {
            As.Play();
        }
    }

    //Función que administra la muerte del personaje.
    private void Muerte()
    {
        if (vida <= 0)
        {
            As.clip = null;
            bx.isTrigger = true;
            rb.simulated = false;
            alive = false;
            if (dirDer == true)
            {
                AnimMachine("Die", 0.01f, 1);
                AsFx.clip = AccionesFx[3];
                AsFx.pitch = 1.1f;
                AsFx.Play();
                AsFx.loop = false;
                As.Stop();
            }
            else
            {
                AnimMachine("Die_volteado", 0.01f, 1);
                AsFx.clip = AccionesFx[3];
                AsFx.pitch = 1.1f;
                AsFx.Play();
                AsFx.loop = false;
                As.Stop();
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class EnemyBrain : MonoBehaviour {

    //Componentes
    public UnityArmatureComponent Armature;
    public DragonBones.AnimationState animación;
    public GameObject RangoAtaque;
    public GameObject ColliderRangoAtaque;
    private GameObject Player;
    public GameObject Colliders;
    public GameObject Vision;
    private Rigidbody2D rb;
    public BoxCollider2D RangoAtaquebx;
    public BoxCollider2D Playerbx;
    public BoxCollider2D visionbx;
    public AudioSource As;
    public AudioSource AsFx;
    public AudioClip[] AccionesFx = new AudioClip[5];


    //Variables que no son compoenentes.
    private bool Avistado = false;
    private bool alive = true;
    public float vida;
    public bool hit = false;
    public float velocidad;
    public bool dirDer = true;
    public int Dir = 1;
    public bool EnMovimiento;
    public bool ModoAtaque = false;
    public float NewChange;
    private float OriginalValue;
    public bool DirSwitch = true;
    public Vector3 movLateral;
    public float DoppelCaminar;


	void Start ()
    {
        //Este primer y segundo bloque son de absoluta impotancia para la administración de las animaciones.
        Armature = this.gameObject.GetComponent<UnityArmatureComponent>();
        animación = Armature._armature.animation.Play("Idle");
        rb = this.GetComponent<Rigidbody2D>();
        RangoAtaque.SetActive(false);

        Armature.AddEventListener(EventObject.LOOP_COMPLETE, AttackCycle);
        Armature.AddEventListener(EventObject.LOOP_COMPLETE, LifeCycle);

        Player = GameObject.Find("Player");
        Playerbx = Player.GetComponent<BoxCollider2D>();
        visionbx = Vision.GetComponent<BoxCollider2D>();
        RangoAtaquebx = RangoAtaque.GetComponent<BoxCollider2D>();
        As = this.GetComponent<AudioSource>();

        //Estos valores se encargan de darle un margen de tiempo al enemigo para cambiar de dirección.
        OriginalValue = NewChange;

	}
	
	void Update ()
    {
        DirectionSwitcher();

        if (alive == true)
        {
            Dirección();
            Move();

            //Esta sección es para la detección del personaje.
            if (visionbx.IsTouching(Playerbx) == true && ModoAtaque == false)
            {
                As.loop = false;
                AsFx.clip = AccionesFx[2];
                AsFx.Play();
                EnMovimiento = true;
                visionbx.enabled = false;
            }
            if (EnMovimiento == true)
            {
                visionbx.enabled = false;
            }
            else
            {
                visionbx.enabled = true;
            }

            if(As.clip== AccionesFx[0])
            {
                As.dopplerLevel = DoppelCaminar;
            }
            else
            {
                As.dopplerLevel = 1;
            }
            
            Die();
        }
        else
        {
            Death();
        }
    }

    //Función encargada del desplazamiento horizontal del enemigo.
    public void Mover()
    {
        movLateral = Vector3.right * velocidad * Dir;
        rb.velocity = movLateral;
    }

    //Función que administra las animaciones de desplazamiento.
    private void Move()
    {
        if (ModoAtaque == false && hit == false)
        {
            if (EnMovimiento == true)
            {
                Mover();
                AnimMachine("Walk");
                if (As.isPlaying == false)
                {
                    As.loop = true;
                    As.clip = AccionesFx[0];
                }
            }
            else
            {
                if (As.isPlaying == false)
                {
                    AnimMachine("Idle");
                    As.loop = true;
                    As.clip = AccionesFx[3];
                }
            }
            if(As.isPlaying == false)
            {
                As.Play();
            }

        }
    }

    //Función encargada de determinar la dirección del enemigo.
    public void Dirección()
    {
        if (dirDer == true)
        {
            Dir = 1;
        }
        else
        {
            Dir = -1;
        }
    }

    //Función para administrar de forma más rápida las animaciones.
    public void AnimMachine(string animName, float FadeIn = 0.1f, int Veces = -1, float timeScale = 1)
    {
        if (animación.animationData.name == animName)
        { }
        else
        {
            Armature._armature.animation.timeScale = timeScale;
            animación = Armature._armature.animation.FadeIn(animName, FadeIn, Veces, 0);
        }
        if(dirDer == true)
        {
            this.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        }
        else
        {
            this.transform.localScale = new Vector3(-1.5f, 1.5f, 1);
        }
    }

    //Función especial que maneja los ciclos de ataque de los enemigos.
    private void AttackCycle(string type, EventObject eventObject)
    {
        switch (type)
        {
            case EventObject.LOOP_COMPLETE:
                if(eventObject.animationState.name == "Attack")
                {
                    RangoAtaque.SetActive(true);
                    AnimMachine("Attack_Retroceso", 0.01f, 1, timeScale: 1.5f);
                }
                else if(eventObject.animationState.name == "Attack_Retroceso")
                {
                    hit = false;
                    ModoAtaque = false;
                    RangoAtaque.SetActive(false);
                    EnMovimiento = true;
                }
                else
                {

                }
                break;
        }
    }

    //Función que determina cómo detecta al protagonista de cerca para atacarlo.
    private void OnTriggerStay2D(Collider2D rangoAtaque)
    {
        if (rangoAtaque.gameObject == Player)
        {
            if (ModoAtaque == false && visionbx.enabled == false)
            {
                EnMovimiento = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
                ModoAtaque = true;
                As.loop = false;
                AsFx.clip = AccionesFx[1];
                AsFx.Play();
                AnimMachine("Attack", 0.01f, 1, timeScale: 1.5f);
            }
        }
    }

    //Función que determina cómo le afecta las balas
    private void OnTriggerEnter2D(Collider2D bala)
    {
        if(bala.gameObject.layer == LayerMask.NameToLayer("Bala"))
        {
            Damage();
            Destroy(bala.gameObject);
        }
    }

    //Función especial que maneja los ciclos de animación que tiene que ver con la vida.
    private void LifeCycle(string type, EventObject eventObject)
    {
        switch (type)
        {
            case EventObject.LOOP_COMPLETE:
                if (eventObject.animationState.name == "Hit")
                {
                    hit = false;
                    EnMovimiento = true;
                    Die();
                }
                break;
        }
    }

    //Función encargada de efectuar la daño recibido.
    private void Damage()
    {
        vida -= 0.25f;
        if (ModoAtaque == false)
        {
            As.loop = false;
            AsFx.clip = AccionesFx[4];
            AsFx.Play();
            AnimMachine("Hit", 0.01f, 1);
            hit = true;
            EnMovimiento = false;
        }
    }

    //Fnción encargada de cambiar de dirección al enemigo.
    public void OnEnemy()
    {
        if (DirSwitch == true)
        {
            if (dirDer == true)
            {
                dirDer = false;
            }
            else
            {
                dirDer = true;
            }
            DirSwitch = false;
        }
    }

    //Función encargada del tiempo entre cambio.
    private void DirectionSwitcher()
    {
        if (NewChange > 0 && DirSwitch == false)
        {
            NewChange -= Time.deltaTime;
        }
        else if (NewChange <= 0)
        {
            if (DirSwitch == false)
            {
                DirSwitch = true;
            }
            else
            {
                NewChange = OriginalValue;
            }
        }
    }

    //Función que desactiva los colliders y el rigidbody tras la muerte del enemigo.
    private void Death()
    {
        rb.simulated = false;
        Destroy(RangoAtaque.gameObject);
        Destroy(Colliders.gameObject);
        Destroy(ColliderRangoAtaque.gameObject);
    }

    //Función encargada de administrar la muerte del enemigo.
    private void Die()
    {
        if (vida <= 0)
        {
            alive = false;
            As.loop = false;
            AsFx.clip = AccionesFx[4];
            AnimMachine("Death", 0.01f, 1);
            AsFx.Play();
        }
    }
}

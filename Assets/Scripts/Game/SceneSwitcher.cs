using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour {

    public bool pausa = false;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Función para generar los cambios de escena para ser usadas mediante botones.
    public void CambioEscena(string escena)
    {
        SceneManager.LoadScene(escena);
    }

    public void CódigoEscene(Text código)
    {
        string code = código.text;
        if (code == "ZAKKB")
        {
            code = "Lvl1 - A";
        }
        else if (code == "TOYFN")
        {
            code = "Lvl1 - B";
        }
        else if (code == "WEGTD")
        {
            code = "Lvl1 - C";
        }
        CambioEscena(code);
    }

    //Función para cerrar el juego para ser usada mediante un botón.
    public void Quit(bool Salir = false)
    {
        if(Salir == true)
        {
            Application.Quit();
        }
    }
    
    //Función que se encarga de, una vez reinciado el nivel en modo Pausa,
    //retorna el tiempo a su valor original.
    public void Pausa(bool EstadoPausa)
    {
        Camera pausa = this.GetComponent<Camera>();
        pausa.Pausa = EstadoPausa;
    }

}

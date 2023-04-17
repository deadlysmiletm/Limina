using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMenú : MonoBehaviour
{
    private AudioSource As;
    public bool Play = false;


	// Use this for initialization
	void Start ()
    {
        As = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Play == true)
        {
            As.Play();
            Play = false;
        }
	}

    public void WillPlay(bool play)
    {
        Play = play;
    }
}

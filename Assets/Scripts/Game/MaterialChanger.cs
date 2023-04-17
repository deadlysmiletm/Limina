using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour {

    public Material SpriteDiffuse;
    private MeshRenderer mr;

	// Use this for initialization
	void Start ()
    {

        mr = this.GetComponent<MeshRenderer>();
        mr.material = SpriteDiffuse;
    }

    // Update is called once per frame
    void Update ()
    {
        mr.material = SpriteDiffuse;
	}
}

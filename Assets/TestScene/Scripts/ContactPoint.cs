using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactPoint : MonoBehaviour {

    Rigidbody rb;

    public int row;
    public int column;

    public float force { get { return rb.velocity.magnitude; } }

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

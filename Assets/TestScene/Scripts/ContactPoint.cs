using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactPoint : MonoBehaviour {

    Rigidbody rb;

    public float force { get { rb.velocity.magnitude; } }

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactPoint : MonoBehaviour {

    private const float _maxForce = 10;

    Rigidbody rb;

    public int row;
    public int column;

    public ushort GetForce
    {
        get
        {
            force = force / _maxForce * ushort.MaxValue;
            return Convert.ToUInt16(Mathf.Clamp(force, ushort.MinValue, ushort.MaxValue));
        }
    }


    public float force { get; set; }

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDrawGizmos()
    {
        
        float percentDistance = /*Mathf.Clamp01*/( Vector3.Distance(randomExplosion.Instance.transform.position, transform.position) / randomExplosion.Instance.radius);
        
        Gizmos.color = Color.Lerp(Color.green, Color.red, percentDistance);
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}

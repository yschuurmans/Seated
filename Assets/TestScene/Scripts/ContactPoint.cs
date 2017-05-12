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
        if (!Application.isPlaying) return;
        float percentDistance = Mathf.Clamp01( Vector3.Distance(randomExplosion.Instance.transform.position, transform.position) / randomExplosion.Instance.radius);
        
        Gizmos.color = new Color(percentDistance * 0.77f + 0.23f, 1 - (1 / percentDistance * 0.77f + 0.23f), 0.23f);
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaFlyer : MonoBehaviour {

    public ContactPoint[] contactPoints;

    void Awake()
    {
        contactPoints = GetComponentsInChildren<ContactPoint>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

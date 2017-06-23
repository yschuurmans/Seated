using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool DebugMode;
    public List<DeltaFlyer> flyers = new List<DeltaFlyer>();
    public static GameManager instance;


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

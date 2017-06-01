using Assets.TestScene.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaFlyer : MonoBehaviour
{
    public Raptor raptor;
    public ContactPoint[] contactPoints { get { return raptor.contactPoints; } }
    public AirStream currentAirStream;

    public bool inAirstream
    {
        get
        {
            if (currentAirStream == null)
                return false;
            else
                return true;
        }
    }

    void Awake()
    {
        raptor = GetComponentInChildren<Raptor>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


}

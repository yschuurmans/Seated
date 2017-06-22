using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderBoost : MonoBehaviour
{

    public float Boost;
    public float MaxBoost;
    public float BoostMultiplier;

    public LiftGlider Glider;
	// Use this for initialization
	void Awake ()
	{
	    Glider = GetComponent<LiftGlider>();
	}
	
	// Update is called once per frame
	void Update () {
	    

    }

    public void AddBoost(float boostToAdd)
    {
        Boost = Mathf.Clamp(Boost + boostToAdd, 0, MaxBoost);
    }

    public void UseBoost(float value)
    {
        if (Boost > value * Time.deltaTime)
        {
            Glider.AddLift(1 * BoostMultiplier * Time.deltaTime * value);
            Boost-= value * Time.deltaTime;
        }
    }
}

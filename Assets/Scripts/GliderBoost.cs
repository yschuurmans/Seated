using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderBoost : MonoBehaviour
{

    public float Boost;
    public float MaxBoost;
    public float BoostMultiplier;
    public Transform BoostBar;
    public float LastBoost;

    private float StartLength;

    private LiftGlider Glider;
    private float boostBonus = 0;
	// Use this for initialization
	void Awake ()
	{
	    Glider = GetComponent<LiftGlider>();
	}

    void Start()
    {
        StartLength = BoostBar.localScale.y;
    }
	// Update is called once per frame
	void Update ()
	{
        if(boostBonus > 1)
            boostBonus -= 0.05f * Time.deltaTime;

        BoostBar.localScale = new Vector3(BoostBar.localScale.x, StartLength * (Boost / MaxBoost), BoostBar.localScale.z);

    }

    public void AddBoost(float boostToAdd)
    {
        Boost = Mathf.Clamp(Boost + boostToAdd, 0, MaxBoost);
    }

    public void UseBoost(float value)
    {
        LastBoost = Time.time;
        if (Boost > value * Time.deltaTime)
        {
            Glider.AddLift(1 * BoostMultiplier * Time.deltaTime * value * boostBonus);
            Boost-= value * Time.deltaTime;
            boostBonus += 0.3f* Time.deltaTime;
        }
    }
}

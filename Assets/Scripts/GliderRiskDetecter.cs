﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderRiskDetecter : MonoBehaviour
{
    public LiftGlider Glider;
    private GliderBoost Boost;

    public float GroundBoostPerSec;
    public float TreeBoost;
    public float minVelocity;
    public float maxVelocity;
    public float SpeedBoostMinVelocity;
    public float SpeedBoostMaxClamp;

    void Start()
    {
        Boost = Glider.Boost;
    }

    void Update()
    {
        if (Glider.Velocity > SpeedBoostMinVelocity)
            Boost.AddBoost(Mathf.Clamp01((Glider.Velocity - SpeedBoostMinVelocity) / (SpeedBoostMaxClamp - SpeedBoostMinVelocity)) * Time.deltaTime);
    }

    void OnTriggerStay(Collider other)
    {
        if (Time.time - Glider.LastBump > 5)
        {
            if (other.CompareTag("Ground")) Boost.AddBoost(GroundBoostPerSec * Time.deltaTime * Mathf.Clamp01((Glider.Velocity - minVelocity) / (maxVelocity - minVelocity)));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (Time.time - Glider.LastBump > 5)
        {
            if (other.CompareTag("Tree")) Boost.AddBoost(TreeBoost);
        }
    }
}

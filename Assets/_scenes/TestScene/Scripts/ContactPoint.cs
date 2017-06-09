using Assets.TestScene.Scripts.HelperClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactPoint : MonoBehaviour
{

    private const float _maxForce = 100;

    Rigidbody rb;

    public int row;
    public int column;

    public ushort GetForce
    {
        get
        {
            float convertedForce = force / _maxForce * ushort.MaxValue;
            return Convert.ToUInt16(Mathf.Clamp(convertedForce, ushort.MinValue, ushort.MaxValue));
        }
    }


    public float force { get; set; }

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = ColorHelper.GetLerpedColor(Color.red, Color.green, GetForce / ushort.MaxValue);
        Gizmos.DrawSphere(transform.position, 0.2f);
    }

    public override string ToString()
    {
        return "[" + row + ", " +column+ "]";
    }
}

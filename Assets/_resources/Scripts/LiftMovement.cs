using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LiftMovement : MonoBehaviour
{

    public Text velText;
    public float LiftMultiplier = 1;
    public float AreaMultiplier = 1;
    public float AirDensity;
    public float Coefficient;

    private Transform tt;
    private Rigidbody rb;
    private Vector3 LocalVelocity;
    private List<Vector3> RecentNormVels;
    private float VelocityLastFrame = 0f;
    private float Velocity = 0f;

    public float Lift;
    public float Area;

    public WingPointTransforms Wings;
    [Serializable]
    public class WingPointTransforms
    {
        public Transform Left;
        public Transform Right;
        public Transform Front;
        public Transform Back;
        
    }


    private void Awake()
    {
        tt = transform;
        rb = GetComponent<Rigidbody>();
    }
    // Use this for initialization
    void Start()
    {
        rb.velocity = Vector3.zero;
        RecentNormVels = new List<Vector3>();
    }

    private float CalculateLift(float velocity, float area)
    {
        // https://www.grc.nasa.gov/www/K-12/airplane/lifteq.html
        //
        //     Cl * ( p * V^2 )                / 2 * A

        return (Coefficient * ((AirDensity * Mathf.Pow(velocity, 2)) / 2) * area) * LiftMultiplier;
    }

    private float CalculateWingArea()
    {
        float width = Vector3.Distance(
            new Vector3(Wings.Left.position.x, 0, Wings.Left.position.z),
            new Vector3(Wings.Right.position.x, 0, Wings.Right.position.z));

        float length = Vector3.Distance(
            new Vector3(Wings.Front.position.x, 0, Wings.Front.position.z),
            new Vector3(Wings.Back.position.x, 0, Wings.Back.position.z));

        //float width = WingPoints.RightWingTip.position.x - WingPoints.LeftWingTip.position.x;
        //float length = WingPoints.FrontWingTip.position.z - WingPoints.BackCenter.position.z;

        return (width * length / 2) * AreaMultiplier;
    }

    private float CalculateFrontalWingArea(Vector3 checkLocation)
    {
        float dWidth = Vector3.Distance(Wings.Left.position, Wings.Right.position);
        float dLength = Vector3.Distance(Wings.Front.position, Wings.Back.position);

        float dLeft = Vector3.Distance(checkLocation, Wings.Left.position);
        float dRight = Vector3.Distance(checkLocation, Wings.Right.position);
        float dFront = Vector3.Distance(checkLocation, Wings.Front.position);
        float dBack = Vector3.Distance(checkLocation, Wings.Back.position);

        float width = dWidth - Mathf.Abs(
            dLeft -
            dRight);

        float length = dLength - Mathf.Abs(
            dFront -
            dBack);

        //float width = WingPoints.RightWingTip.position.x - WingPoints.LeftWingTip.position.x;
        //float length = WingPoints.FrontWingTip.position.z - WingPoints.BackCenter.position.z;
        float area = (width * length / 2) / (dWidth * dLength);
        return Mathf.Lerp(0.1f, 1f, area) + AreaMultiplier;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Area = CalculateWingArea();
        RecentNormVels.Add(rb.velocity.normalized);
        while (RecentNormVels.Count > 30) RecentNormVels.RemoveAt(0);

        Vector3 avg = Vector3.zero;
        RecentNormVels.ForEach(vec => avg += vec);
        avg = avg / RecentNormVels.Count;

        Area = CalculateFrontalWingArea(avg * 5);

        Velocity = rb.velocity.magnitude;
        Lift = CalculateLift(transform.InverseTransformDirection(rb.velocity).z, Area);



        float angle = Mathf.Clamp((Vector3.Angle(tt.up, rb.velocity) / 90f - 1f)*2, -1f, 1f);
        LocalVelocity = (angle * tt.up) * Lift;

        //LocalVelocity = tt.up * Lift;

        //LocalVelocity = rb.velocity.normalized*-1 * Lift;

        //LocalVelocity = tt.rotation * LocalVelocity;
        rb.AddForce(LocalVelocity);
        //rb.AddForce(tt.forward * (Velocity - VelocityLastFrame));



        velText.text = "Area: " + Area + "\nLift: " + Lift.ToString("F1") + "\nSpeed: " + Velocity.ToString("F1") + "\nAcceleration: "+ (Velocity-VelocityLastFrame)/Time.fixedDeltaTime + "\nHeight: " + tt.position.y.ToString("F1");

        VelocityLastFrame = Velocity;

        //LocalVelocity = Vector3.Lerp(
        //    rb.velocity - rb.velocity * 0.002f * Time.fixedDeltaTime,
        //    LocalVelocity,
        //    5 * Time.fixedDeltaTime)
        //    + Physics.gravity * rb.mass;

        //rb.velocity = LocalVelocity;





        //tt.position = new Vector3(0, 50, 0);

        //Debug.Log("angle:" + Mathf.Abs(tt.eulerAngles.x < 180 ? tt.eulerAngles.x : tt.eulerAngles.x - 360f));

        //float angleX = Mathf.Abs(tt.eulerAngles.x < 180 ? tt.eulerAngles.x : tt.eulerAngles.x - 360f);
        //float angleZ = Mathf.Abs(tt.eulerAngles.z < 180 ? tt.eulerAngles.z : tt.eulerAngles.z - 360f);
        //if (angleX < PitchResistance && angleZ < RollResistance)
        //{
        //    tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.Euler(0, tt.eulerAngles.y, tt.eulerAngles.z), (1 - angleX / PitchResistance) * Time.deltaTime);
        //    tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.Euler(tt.eulerAngles.x, tt.eulerAngles.y, 0), (1 - angleZ / RollResistance) * Time.deltaTime);
        //}
        //else if (angleX > 180 - PitchResistance && angleZ > 180 - RollResistance)
        //{
        //    tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.Euler(180, tt.eulerAngles.y, tt.eulerAngles.z), (1 - angleX / PitchResistance) * Time.deltaTime);
        //    tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.Euler(tt.eulerAngles.x, tt.eulerAngles.y, 180), (1 - angleZ / RollResistance) * Time.deltaTime);
        //}



    }


    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rb.position, rb.position + rb.velocity);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(rb.position, rb.position + LocalVelocity);
        }
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(rb.position, rb.position + transform.InverseTransformDirection(rb.velocity));
    }
}

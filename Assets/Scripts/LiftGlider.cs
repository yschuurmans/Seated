using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LiftGlider : MonoBehaviour
{
    public float LiftMultiplier = 0.75f;
    public float UpwardLiftRatio = 1f;
    public float ForwardLiftRatio = 3f;
    public float GravModifierImpact = 1f;
    public float LiftGravModifier = 1f;

    public Text text;

    private Transform tt;
    private Rigidbody rb;
    public float Lift;
    private float GravModifier;
    private Vector3 Direction;
    private Vector3 LocalVelocity;
    private List<float> RecentLift;


    private void Awake()
    {
        tt = transform;
        rb = GetComponent<Rigidbody>();
        RecentLift = new List<float>();
    }
    // Use this for initialization
    void Start()
    {
    }

    private float CalculateLift(float velocity)
    {
        // https://www.grc.nasa.gov/www/K-12/airplane/lifteq.html
        //
        //     Cl * ( p * V^2 )                / 2 * A
        //return 1 * (1 * Mathf.Pow(velocity, 2)) / 2 * 1;

        return Mathf.Clamp(Mathf.Pow(velocity, 5f / 7f) * 3, 0, Mathf.Max(5 * velocity,1000));
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        RecentLift.Add(CalculateLift(rb.velocity.magnitude));
        while (RecentLift.Count > 60) RecentLift.RemoveAt(0);
        Lift = RecentLift.Average();

        Debug.Log("Velocity: " + rb.velocity.magnitude + " Lift: " + Lift);
        LocalVelocity = Vector3.zero;

        LocalVelocity.y = Lift * UpwardLiftRatio;
        LocalVelocity.z = Lift * ForwardLiftRatio;
        LocalVelocity = LocalVelocity.normalized * Lift * LiftMultiplier;
        

        LocalVelocity = tt.rotation * LocalVelocity;

        GravModifier = Mathf.Lerp(0.1f, 0.5f, (Mathf.Clamp(Vector3.Angle(Vector3.down, -1*tt.up), 0, 110) / 110));


        LocalVelocity = Vector3.Lerp(rb.velocity - rb.velocity * 0.2f * Time.deltaTime,
            LocalVelocity,
            5f * Time.deltaTime) + (Physics.gravity * rb.mass * GravModifier);

        rb.velocity = LocalVelocity;


        text.text = "Lift: " + Lift.ToString("F1") + "\nHeight: " + tt.position.y.ToString("F1");


        #region nonWorking
        /*
        //LiftGravModifier = 1- Mathf.Abs(1 - (Vector3.Angle(Vector3.down, tt.forward) / 90));
        LiftGravModifier = Mathf.Clamp01(0.2f + Mathf.Clamp(Vector3.Angle(Vector3.up, tt.forward) - 45, 0, 130) / 130);

        Lift = Mathf.Lerp(Lift - Lift * 0.05f, CalculateLift(rb.velocity.magnitude * LiftGravModifier), 0.1f);


        Debug.Log("Velocity: " + rb.velocity.magnitude + " Lift: " + Lift);
        LocalVelocity = Vector3.zero;

        LocalVelocity += UpwardLiftRatio * tt.up;
        LocalVelocity += ForwardLiftRatio * tt.forward;
        LocalVelocity = LocalVelocity.normalized * Lift * LiftMultiplier;

        text.text = LocalVelocity.ToString();

        rb.AddForce(LocalVelocity);
        */
        #endregion

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rb.position, rb.position + rb.velocity);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(rb.position, rb.position + LocalVelocity);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(rb.position, rb.position + Physics.gravity * rb.mass * GravModifier
            /* * (GravModifier * GravModifierImpact)*/);
    }



    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(tt.position, tt.position + tt.forward * 5);

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(tt.position, tt.position + tt.right * 5);

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawLine(tt.position, tt.position + tt.up * 5);
    //}
}

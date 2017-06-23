using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    public float LastBump = -5;

    public GliderBoost Boost;

    public Text debugText1;
    public Text debugText2;

    private Transform tt;
    private Rigidbody rb;
    public float Lift;
    public float Velocity;
    private float GravModifier;
    private Vector3 Direction;
    private Vector3 LocalVelocity;
    private List<float> RecentLift;


    private void Awake()
    {
        tt = transform;
        rb = GetComponent<Rigidbody>();
        RecentLift = new List<float>();
        Boost = GetComponent<GliderBoost>();
    }
    // Use this for initialization
    void Start()
    {
        if (!GameManager.instance.DebugMode)
        {
            //debugText2.gameObject.SetActive(false);
            //debugText1.gameObject.SetActive(false);
        }
    }

    private float CalculateLift(float velocity)
    {
        // https://www.grc.nasa.gov/www/K-12/airplane/lifteq.html
        //
        //     Cl * ( p * V^2 )                / 2 * A
        //return 1 * (1 * Mathf.Pow(velocity, 2)) / 2 * 1;

        return Mathf.Clamp(Mathf.Pow(velocity, 5f / 7f) * 3, 0, Mathf.Max(5 * velocity, 1000));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Velocity = rb.velocity.magnitude;
        LiftGravModifier = 0.5f;
        //LiftGravModifier = Mathf.Lerp(Mathf.Clamp01(0.2f + Mathf.Clamp(Vector3.Angle(Vector3.up, tt.forward) - Mathf.Clamp(Lift / 2,0,45), 0, 180) / 180), 0.7f, Mathf.Clamp01(Mathf.Pow(Lift / 60, 0.2f)));
        RecentLift.Add(CalculateLift(rb.velocity.magnitude * LiftGravModifier));
        while (RecentLift.Count > 60) RecentLift.RemoveAt(0);
        Lift = RecentLift.Average();

        //Debug.Log("Velocity: " + rb.velocity.magnitude + " Lift: " + Lift);
        LocalVelocity = Vector3.zero;

        LocalVelocity.y = Lift * UpwardLiftRatio;// * Mathf.Clamp01(Mathf.Pow(Lift / 15, 3f));
        LocalVelocity.z = Lift * ForwardLiftRatio * Mathf.Clamp01(Mathf.Pow(Lift / 20, 0.3f));
        LocalVelocity = LocalVelocity.normalized * Lift * LiftMultiplier;


        // Lift Degradation flying sideways: https://www.desmos.com/calculator/rw5hcjrbwo

        float forwardGravMod = 1 - Mathf.Clamp(Vector3.Angle(Vector3.down, tt.forward), 0, 70) / 70;
        float downwardGravMod =
            Mathf.Clamp01(Mathf.Clamp(Vector3.Angle(Vector3.down, tt.up * -1), 0, 90) / 90 *
                          Mathf.Clamp01(Mathf.Pow(Lift / 20, 0.2f)));
        float liftGravMod = 1 - Mathf.Clamp01(Mathf.Pow(Lift / 20, 0.2f));

        GravModifier = (forwardGravMod + downwardGravMod + liftGravMod * 2) / 4;

        LocalVelocity = tt.rotation * LocalVelocity;

        LocalVelocity = Vector3.Lerp(rb.velocity - rb.velocity * 0.2f * Time.deltaTime,
            LocalVelocity,
            5f * Time.deltaTime);

        LocalVelocity = Vector3.Lerp(LocalVelocity, rb.velocity, GravModifier * Mathf.Clamp01(Velocity / (Lift * LiftMultiplier) - 1));


        LocalVelocity += Physics.gravity * rb.mass * (GravModifier * GravModifierImpact);

        if (!float.IsNaN(LocalVelocity.x))
            rb.velocity = LocalVelocity;

        if (GameManager.instance.DebugMode)
        {
            debugText1.text = "Boost: " + Boost.Boost;
            debugText2.text = "Velocity: " + Velocity;
            //debugText2.text =
            //forwardGravMod + "\n" +
            //downwardGravMod + "\n" +
            //liftGravMod * 2 + "\n" +
            //"/4 = " + GravModifier + "\n\n" +
            //Mathf.Clamp01(Mathf.Pow(Lift / 30, 3f)) + "\n";

            //debugText1.text = "Lift: " + Lift.ToString("F1") + "\nHeight: " + tt.position.y.ToString("F1") + "\nVelocity: " + Velocity.ToString("F1") + "\n DownwardMod: " + Mathf.Clamp01(Velocity / (Lift * LiftMultiplier) - 1);
        }

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

        debugText1.debugText1 = LocalVelocity.ToString();

        rb.AddForce(LocalVelocity);
        */
        #endregion

    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rb.position, rb.position + rb.velocity);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(rb.position, rb.position + LocalVelocity);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(rb.position, rb.position + Physics.gravity * rb.mass * GravModifier
                /* * (GravModifier * GravModifierImpact)*/);

        }
    }

    public void AddLift(float value)
    {
        if (RecentLift.Count <= 0) return;
        RecentLift.Add(RecentLift[RecentLift.Count - 1] + value);
    }



    void OnCollisionEnter(Collision other)
    {
        LastBump = Time.time;
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LiftMovement : MonoBehaviour
{
    public float LiftMultiplier = 0.75f;
    public float UpwardLiftRatio =1f;
    public float ForwardLiftRatio = 3f;
    public float GravModifierImpact = 1f;


    private Transform tt;
    private Rigidbody rb;
    private float Lift;
    private List<float> RecentLift;
    private float LiftGravModifier;
    private float VelGravModifier;
    private Vector3 Direction;
    private Vector3 LocalVelocity;

    public float lift;
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

        return Mathf.Clamp(Mathf.Pow(velocity, 5f / 6f) * 3,0, 4*velocity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //LiftGravModifier = 1- Mathf.Abs(1 - (Vector3.Angle(Vector3.down, tt.forward) / 90));
        LiftGravModifier = Mathf.Clamp01(0.2f+ Mathf.Clamp(Vector3.Angle(Vector3.up, tt.forward)-45, 0, 130) / 130);
        RecentLift.Add(CalculateLift(rb.velocity.magnitude * LiftGravModifier));
        while (RecentLift.Count > 60) RecentLift.RemoveAt(0);
        Lift = RecentLift.Average();

        Debug.Log("Velocity: " + rb.velocity.magnitude + " Lift: " + Lift);
        LocalVelocity = Vector3.zero;

        LocalVelocity.y = Lift * UpwardLiftRatio;
        LocalVelocity.z = Lift * ForwardLiftRatio;
        LocalVelocity = LocalVelocity.normalized * Lift * LiftMultiplier;


        VelGravModifier = 1 - (Mathf.Clamp(Vector3.Angle(Vector3.down, tt.forward), 0, 90) / 90);

        LocalVelocity = tt.rotation * LocalVelocity;

        LocalVelocity = Vector3.Lerp(rb.velocity - rb.velocity * 0.2f * Time.deltaTime,
            LocalVelocity,
            5f * Time.deltaTime) + Physics.gravity * rb.mass * (VelGravModifier * GravModifierImpact);

        rb.velocity = LocalVelocity;

        //tt.position = new Vector3(0, 50, 0);

    }

    private List<Vector3> recentVelocitys = new List<Vector3>();
    private List<Vector3> recentLocalVel = new List<Vector3>();
    void OnDrawGizmos()
    {
        recentVelocitys.Add(rb.velocity);
        while (recentVelocitys.Count > 10) recentVelocitys.RemoveAt(0);

        Gizmos.color = Color.blue;
        Vector3 avg = Vector3.zero;
        recentVelocitys.ForEach(x => avg += x);
        avg = avg / recentVelocitys.Count;
        Gizmos.DrawLine(rb.position, rb.position + avg * 5);


        LocalVelocity.y = Lift * UpwardLiftRatio;
        LocalVelocity.z = Lift * ForwardLiftRatio;
        LocalVelocity = Vector3.Lerp(rb.velocity, LocalVelocity, 0.2f);
        LocalVelocity = tt.localRotation * LocalVelocity;

        recentLocalVel.Add(LocalVelocity);
        while (recentLocalVel.Count > 10) recentLocalVel.RemoveAt(0);

        Gizmos.color = Color.red;
        Vector3 avg2 = Vector3.zero;
        recentLocalVel.ForEach(x => avg2 += x);
        avg2 = (avg2 / recentLocalVel.Count);
        Gizmos.DrawLine(rb.position, rb.position + avg2 * 5);
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

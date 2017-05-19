using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LiftMovement : MonoBehaviour
{
    public float LiftMultiplier;
    public float ForwardLiftMultiplier;

    private Transform tt;
    private Rigidbody rb;
    private float Lift;
    private Vector3 Direction;
    private Vector3 LocalVelocity;
    private void Awake()
    {
        tt = transform;
        rb = GetComponent<Rigidbody>();
    }
    // Use this for initialization
    void Start()
    {
    }

    private float CalculateLift(float velocity)
    {
        // https://www.grc.nasa.gov/www/K-12/airplane/lifteq.html
        //     Cl * ( p * V^2 )                / 2 * A
        return 1 * (1 * Mathf.Pow(velocity, 2)) / 2 * 1;
    }

    // Update is called once per frame
    void Update()
    {
        Lift = CalculateLift(rb.velocity.magnitude);
        Debug.Log("Velocity: " + rb.velocity.magnitude + " Lift: " + Lift);
        LocalVelocity = Vector3.zero;

        LocalVelocity.y = Lift * LiftMultiplier;
        LocalVelocity.z = Lift * ForwardLiftMultiplier;
        LocalVelocity = Vector3.Lerp(rb.velocity - rb.velocity * 0.1f * Time.deltaTime,
            LocalVelocity.x * tt.right + LocalVelocity.y * tt.up + LocalVelocity.z * tt.forward,
            0.2f * Time.deltaTime);


        //rb.velocity = LocalVelocity;

        tt.position = new Vector3(0, 50, 0);

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


        LocalVelocity.y = Lift * LiftMultiplier;
        LocalVelocity.z = Lift * ForwardLiftMultiplier;
        LocalVelocity = Vector3.Lerp(rb.velocity, LocalVelocity, 0.2f);
        LocalVelocity = tt.localRotation * LocalVelocity;

        recentLocalVel.Add(LocalVelocity);
        while (recentLocalVel.Count > 10) recentLocalVel.RemoveAt(0);

        Gizmos.color = Color.red;
        Vector3 avg2 = Vector3.zero;
        recentVelocitys.ForEach(x => avg2 += x);
        avg2 = avg2 / recentVelocitys.Count;
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

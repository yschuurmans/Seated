using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LiftMovement : MonoBehaviour
{
    public Text velText;
    public float LiftMultiplier = 0.75f;
    public float UpwardLiftRatio = 1f;
    public float ForwardLiftRatio = 3f;
    public float GravModifierImpact = 1f;
    public float PitchResistance;
    public float RollResistance;


    private Transform tt;
    private Rigidbody rb;
    public float Lift;
    private List<float> RecentLift;
    private float LiftGravModifier;
    private float VelGravModifier;
    private Vector3 Direction;
    private Vector3 LocalVelocity;
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

        return Mathf.Clamp(Mathf.Pow(velocity, 4f / 6f) * 3, velocity / 2, 8 * velocity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //LiftGravModifier = 1- Mathf.Abs(1 - (Vector3.Angle(Vector3.down, tt.forward) / 90));

        //RecentLift.Add(CalculateLift(rb.velocity.magnitude * LiftGravModifier));
        ////RecentLift.Add((CalculateLift(rb.velocity.magnitude * LiftGravModifier) + Lift) / 2);
        //while (RecentLift.Count > 60) RecentLift.RemoveAt(0);
        //Lift = RecentLift.Average();




        LiftGravModifier = Mathf.Clamp01(0.2f + Mathf.Clamp(Vector3.Angle(Vector3.up, tt.forward), 0, 70) / 70);

        Lift = CalculateLift(rb.velocity.magnitude * LiftGravModifier);


        LocalVelocity = Vector3.zero;

        LocalVelocity.y = UpwardLiftRatio;
        LocalVelocity.z = ForwardLiftRatio;
        LocalVelocity = LocalVelocity.normalized * Lift * LiftMultiplier;

        float downwardAngle = Vector3.Angle(Vector3.down, tt.forward);

        VelGravModifier = (Mathf.Clamp(downwardAngle, 0, 90) / 90);

        LocalVelocity = tt.rotation * LocalVelocity;

        LocalVelocity = Vector3.Lerp(
            rb.velocity - rb.velocity * 0.002f /** Time.fixedDeltaTime*/,
            LocalVelocity,
            5 * Time.fixedDeltaTime) 
            + Physics.gravity * rb.mass * (VelGravModifier * GravModifierImpact);

        rb.velocity = LocalVelocity;





        //tt.position = new Vector3(0, 50, 0);

        Debug.Log("angle:" + Mathf.Abs(tt.eulerAngles.x < 180 ? tt.eulerAngles.x : tt.eulerAngles.x - 360f));

        float angleX = Mathf.Abs(tt.eulerAngles.x < 180 ? tt.eulerAngles.x : tt.eulerAngles.x - 360f);
        float angleZ = Mathf.Abs(tt.eulerAngles.z < 180 ? tt.eulerAngles.z : tt.eulerAngles.z - 360f);
        if (angleX < PitchResistance && angleZ < RollResistance)
        {
            tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.Euler(0, tt.eulerAngles.y, tt.eulerAngles.z), (1 - angleX / PitchResistance) * Time.deltaTime);
            tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.Euler(tt.eulerAngles.x, tt.eulerAngles.y, 0), (1 - angleZ / RollResistance) * Time.deltaTime);
        }
        else if (angleX > 180 - PitchResistance && angleZ > 180 - RollResistance)
        {
            tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.Euler(180, tt.eulerAngles.y, tt.eulerAngles.z), (1 - angleX / PitchResistance) * Time.deltaTime);
            tt.rotation = Quaternion.Lerp(tt.rotation, Quaternion.Euler(tt.eulerAngles.x, tt.eulerAngles.y, 180), (1 - angleZ / RollResistance) * Time.deltaTime);
        }



        velText.text = "Lift: " + Lift.ToString("F1") + "\nSpeed: " + rb.velocity.magnitude.ToString("F1") + "\nHeight: " + tt.position.y.ToString("F1") + "\nAngle: " + angleX.ToString("F1");
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

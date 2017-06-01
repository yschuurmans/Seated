using Assets.TestScene.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeltaFlyer : MonoBehaviour
{
    public Raptor raptor;
    public ContactPoint[] contactPoints { get { return raptor.contactPoints; } }
    public AirStream currentAirStream;
    public AirStream detectedAirStream;
    public ParticleSystem ps;

    private Transform movingDirection;



    private float minImpactRadius = 0.8f;
    private float maxImpactRadius = 1.3f;

    //gizmos variables
    public float impactRadius;
    Vector3 gizClosesImpactPoint;

    /// <summary>
    /// Whether this Deltaflyer is or is not in an Airstream
    /// </summary>
    public bool isInAirstream
    {
        get
        {
            if (currentAirStream == null)
                return false;
            else
                return true;
        }
    }

    void Awake()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        raptor = GetComponentInChildren<Raptor>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void LateUpdate()
    {
        if (currentAirStream != null) ps.transform.position = currentAirStream.getClosestPoint(this);
        ps.gameObject.transform.LookAt(movingDirection);
        ps.transform.Rotate(0, 180, 0);
        if (currentAirStream != null) ps.transform.position = ps.transform.position + ps.transform.forward * -150;
    }

    public void resetMotors()
    {
        foreach (ContactPoint cp in contactPoints)
        {
            cp.force = 0;
        }
    }


    public void enableSpeed(Transform movingDirection)
    {
        this.movingDirection = movingDirection;
        var em = ps.emission;
        em.enabled = true;
    }

    public void disableSpeed()
    {
        movingDirection = null;
        var em = ps.emission;
        em.enabled = false;
    }

    /// <summary>
    /// Action when Deltaflyer detects an Airstream
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="closestPointOnLine"></param>
    public void detectAirstream(AirStream stream, Vector3 closestPointOnLine)
    {
        detectedAirStream = stream;
        RaycastHit hit;
        Vector3 direction = (this.raptor.raptorCollider.transform.position - closestPointOnLine).normalized;
        Ray ray = new Ray(closestPointOnLine, direction);
        Physics.Raycast(ray, out hit);
        Vector3 closestImpactPoint = hit.point;
        gizClosesImpactPoint = closestImpactPoint;

        float dist = Vector3.Distance(closestPointOnLine, closestImpactPoint) - stream.thickness;
        float range = stream.thickness * stream._AlertMultiplyer - stream.thickness;
        float distPerc = (100 - (dist / (range / 100))) / 100;

        float radiusRange = maxImpactRadius - minImpactRadius;

        float radius = (radiusRange * distPerc) + minImpactRadius;
        impactRadius = radius;


        List<ContactPoint> points = contactPoints.Where(p => Vector3.Distance(p.transform.position, closestImpactPoint) <= radius).ToList();

        foreach (ContactPoint c in points)
        {
            c.force = getForce(stream.force, c.transform.position, closestImpactPoint, stream._MinAlertForcePerc, radius);
        }
    }

    /// <summary>
    /// Action when Deltaflyer is in an Airstream
    /// </summary>
    /// <param name="closestPoint"></param>
    public void inAirstream(Vector3 closestPoint)
    {
        resetMotors();
    }

    public float getForce(float force, Vector3 objectPos, Vector3 closestPointOnLine, float minForcePerc, float range)
    {
        float dist = Vector3.Distance(objectPos, closestPointOnLine);
        float perc = (100 - (dist / (range / 100))) / 100;

        float minForce = force * (minForcePerc / 100);
        float tempForce = force - minForce;
        float newForce = tempForce * perc;
        float totalForce = newForce + minForce;

        return totalForce;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gizClosesImpactPoint, impactRadius);
    }


}

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
    //only one airstream can be detect at a time
    //thats why theres 1 detectedairstream + a list of detectedairstreams
    public AirStream detectedAirStream;
    public List<AirStream> detectedAirStreams = new List<AirStream>();
    InputManager inputMngr;   

    private float minImpactRadius = 0.8f;
    private float maxImpactRadius = 1.5f;

    //gizmos variables
    public float impactRadius;
    Vector3 gizClosesImpactPoint;
    public Vector3 tmpPoint;
    public Vector3 tmpClostestPoint;


    //testVariables
    public int detectedAirstreamsCount = 0;

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

    public bool isInDetectionRange
    {
        get
        {
            if (detectedAirStream == null)
                return false;
            else
                return true;
        }
    }

    void Awake()
    {
        inputMngr = GetComponent<InputManager>();
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
        //if (currentAirStream != null) placeParticleSystem(currentAirStream, movingDirection, currentAirStream.getClosestPoint(this), ps);
    }

    public void resetMotors()
    {
        foreach (ContactPoint cp in contactPoints)
        {
            cp.force = 0;
        }
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
        //resetMotors();
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

    /// <summary>
    /// Frame when Deltaflyer entered an airstream
    /// </summary>
    /// <param name="movingDirection"></param>
    public void enteredAirstream(AirStream stream)
    {
        Debug.Log("entered an airstream");
        stream.inAirstream.Add(this);
        currentAirStream = stream;
        detectedAirStream = stream;

        Transform movingDir = stream.getMovingToPoint(this);
        foreach (AirStream tempStream in detectedAirStreams)
        {
            tempStream.enterParticleStream(this, stream.getOtherPoint(movingDir));
        }
        detectedAirStream.enterParticleStream(this, movingDir);

        inputMngr.velocity *= 2;        

        foreach (ContactPoint cp in contactPoints)
        {
            cp.force = 40;
        }

        //foreach (AirStream tmpAs in detectedAirStreams)
        //{
        //    tmpAs.notifyParticles.gameObject.SetActive(true);
        //}
    }

    /// <summary>
    /// Frame when Deltaflyer left an airstream
    /// </summary>
    public void leftAirstream()
    {
        foreach(AirStream stream in detectedAirStreams)
        {
            stream.leaveParticleStream(this);
        }

        currentAirStream.leaveParticleStream(this);
        currentAirStream.inAirstream.Remove(this);
        currentAirStream = null;

        inputMngr.velocity /= 2;

        //foreach (AirStream tmpAs in detectedAirStreams)
        //{
        //    tmpAs.notifyParticles.gameObject.SetActive(false);
        //}

    }

    public void enteredDetectionRange(AirStream stream)
    {

        if (!isInDetectionRange)
        {
            Debug.Log("user is not in detection range yet, so entering detection range");
            detectedAirStream = stream;
        }
        else if (!isInAirstream && detectedAirStream != stream)
        {
            if (stream.isClostestAirstream(this, stream, detectedAirStream))
            {
                Debug.Log("user detected an airstream that is closest. user is not in an airstream");
                detectedAirStream = stream;
            }
            else
            {
                Debug.Log("user detected an airstream but it is NOT closest. user is not in an airstream");
            }
        }
        if (isInAirstream && !detectedAirStreams.Contains(stream))
        {
            Debug.Log("user is in an airstream and detected by another airstream");
            stream.enterParticleStream(this, stream.getOtherPoint(detectedAirStream.ps.transform));
        }

        detectedAirStreams.Add(stream);
        stream.inDetectionRange.Add(this);
        Debug.Log("Airstream entered, count: " + detectedAirStreams.Count);
        
        detectedAirstreamsCount++;
}

    public void leftDetectionRange(AirStream stream)
    {
        if (!isInAirstream) { resetMotors(); }
        if (detectedAirStream == stream) { detectedAirStream = null; }
        detectedAirStreams.Remove(stream);
        stream.inDetectionRange.Remove(this);
        stream.leaveParticleStream(this);
        Debug.Log("Airstream left, count: " + detectedAirStreams.Count);
        
        detectedAirstreamsCount--;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gizClosesImpactPoint, impactRadius);
        if (currentAirStream != null)
        {
            Vector3 tempClosestPointOnLine = currentAirStream.getClosestPoint(transform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(tempClosestPointOnLine, currentAirStream.thickness);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(tempClosestPointOnLine, currentAirStream.thickness * currentAirStream._AlertMultiplyer);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(tempClosestPointOnLine, 0.3f);
        }

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(tmpClostestPoint, 1f);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(tmpPoint, 1f);
    }


}

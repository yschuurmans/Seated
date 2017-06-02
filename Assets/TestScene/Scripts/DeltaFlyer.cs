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
    InputManager inputMngr;

    private Transform movingDirection;

    private AirStream particleStream;
    public List<AirStream> detectedAirStreams = new List<AirStream>();
    List<ParticleSystem> psPool = new List<ParticleSystem>();

    private float minImpactRadius = 0.8f;
    private float maxImpactRadius = 1.3f;

    //gizmos variables
    public float impactRadius;
    Vector3 gizClosesImpactPoint;
    public Vector3 tmpPoint;
    public Vector3 tmpClostestPoint;

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
        updateParticleSystem();
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
        movingDirection = currentAirStream.getMovingToPoint(this);
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
        Debug.Log("Entering an airstream: " + stream.ToString());
        currentAirStream = stream;
        particleStream = stream;

        inputMngr.velocity *= 2;

        this.movingDirection = stream.getMovingToPoint(this);
        var em = ps.emission;
        em.enabled = true;

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
        Debug.Log("Leaving an airstream: " + currentAirStream.ToString());
        currentAirStream = null;

        inputMngr.velocity /= 2;

        movingDirection = null;
        var em = ps.emission;
        em.enabled = false;

        //foreach (AirStream tmpAs in detectedAirStreams)
        //{
        //    tmpAs.notifyParticles.gameObject.SetActive(false);
        //}

    }

    public void enteredDetectionRange(AirStream stream)
    {
        detectedAirStream = stream;
    }

    public void leftDetectionRange(AirStream stream)
    {
        if (detectedAirStreams.Contains(detectedAirStream)) detectedAirStream = null;
        else detectedAirStreams.Remove(stream);
        if (!isInAirstream) resetMotors();
    }

    public void addMovingDirection(AirStream stream)
    {
        if (!detectedAirStreams.Contains(stream))
        {
            //ParticleSystem sys = getParticleSystem();
            Transform movDir = stream.getOtherPoint(movingDirection);
            //stream.notifyParticles = sys;
            detectedAirStreams.Add(stream);
        }
    }

    void updateParticleSystem()
    {
        if (movingDirection != null)
        {
            if (Vector3.Distance(ps.transform.position, movingDirection.transform.position) < 5)
            {
                if (detectedAirStreams[0] != null)
                {
                    particleStream = detectedAirStreams[0];
                    movingDirection = particleStream.getOtherPoint(movingDirection);
                }
            }


            if (particleStream != null)
            {

                ps.transform.position = currentAirStream.getClosestPoint(transform.position);
                ps.gameObject.transform.LookAt(movingDirection);
                ps.transform.Rotate(0, 180, 0);
                Vector3 point = ps.transform.position + ps.transform.forward * -150;

                tmpPoint = point;


                ps.transform.position = particleStream.getClosestPoint(point);
                tmpClostestPoint = ps.transform.position;

                if (particleStream == currentAirStream) ps.gameObject.transform.LookAt(movingDirection);
                else ps.gameObject.transform.LookAt(particleStream.getOtherPoint(movingDirection));

                ps.transform.Rotate(0, 180, 0);

                //ps.transform.position += ps.transform.forward * -150;
            }
        }
    }

    private ParticleSystem getParticleSystem()
    {
        foreach (ParticleSystem sys in psPool)
        {
            if (!sys.gameObject.activeSelf)
            {
                sys.gameObject.SetActive(true);
                return sys;
            }
        }

        ParticleSystem newSys = Instantiate<ParticleSystem>(ps);
        psPool.Add(newSys);
        newSys.gameObject.SetActive(true);
        return newSys;
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

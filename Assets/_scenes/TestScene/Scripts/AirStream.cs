using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AirStream : MonoBehaviour
{
    public int _AlertMultiplyer = 10;
    public float thickness;
    public Transform startPoint;
    public Transform endPoint;


    public float _MaxAlertForcePerc = 40;
    public float _MinAlertForcePerc = 5;
    public Vector3 closestPointOnLineTemp;
    public float force;
    private List<ContactPoint> contactPointsHit = new List<ContactPoint>();
    private List<ContactPoint> knownContactPoints = new List<ContactPoint>();

    public ParticleSystem ps;
    SoundManager sm;

    public List<DeltaFlyer> inAirstream = new List<DeltaFlyer>();
    public List<DeltaFlyer> inDetectionRange = new List<DeltaFlyer>();



    public bool alwaysDrawGizmos;

    void Awake()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        ps.Stop();
    }

    // Use this for initialization
    void Start()
    {
        startPoint.LookAt(endPoint);
        startPoint.transform.Rotate(90, 0, 0);

        endPoint.LookAt(startPoint);
        endPoint.transform.Rotate(90, 0, 0);

        sm = FindObjectOfType<SoundManager>();
    }


    // Update is called once per frame
    void Update()
    {
        foreach (DeltaFlyer df in GameManager.instance.flyers)
        {
            Vector3 closestPoint = ClosestPointOnLine(startPoint.position, endPoint.position, df.transform.position);
            float dist = Vector3.Distance(closestPoint, df.transform.position);

            if (df.currentAirStream == this && dist > thickness)
            {
                //leaving this AirStream
                df.leftAirstream();
            }
            else if (df.detectedAirStreams.Contains(this) && dist > (thickness * _AlertMultiplyer))
            {
                //leaving this Detection Range
                df.leftDetectionRange(this);
            }                
            else if (dist < thickness)
            {
                if (!df.isInAirstream || (df.currentAirStream != this && !isClostestAirstream(df, this, df.currentAirStream)))
                {
                    if (df.currentAirStream != null)
                    {
                        //Entering this airstream, makes the deltaflyer leave the other airstream
                        df.leftAirstream();
                    }

                    //entering this Airstream
                    df.enteredAirstream(this);
                }

                if (df.currentAirStream == this)
                {
                    //deltaflyer is in this airStream
                    //Action done when this Deltaflyer is in This airstream
                    df.inAirstream(closestPoint);
                }
            }
            else if (dist < (thickness * _AlertMultiplyer))
            {
               
                if (!df.isInDetectionRange || !df.detectedAirStreams.Contains(this))
                {
                    //if (df.currentAirStream != null)
                    //{
                    //    //Entering this DetectionRange, makes the deltaflyer leave the other detectionRange
                    //    df.leftDetectionRange(this);
                    //}

                    //entering this DetectionRange   
                    df.enteredDetectionRange(this);
                }

                if (df.detectedAirStream == this && !df.isInAirstream)
                {
                    //Deltaflyer is near this airStream
                    //Action done when this Deltaflyer is near this airstream
                    df.detectAirstream(this, closestPoint);
                }
            }
              
             
        }
    }

    public void enterParticleStream(DeltaFlyer df, Transform movingToPoint)
    {
        ps.transform.position = getOtherPoint(movingToPoint).position;
        ps.transform.LookAt(movingToPoint);
        ps.Play();
        sm.PlayWind();
    }

    public void leaveParticleStream(DeltaFlyer df)
    {
        ps.Stop();
        if (!df.isInAirstream) sm.StopWind();
    }

    public Transform getMovingToPoint(DeltaFlyer df)
    {
        float startAngle = Vector3.Angle(startPoint.position, df.transform.forward);
        float endAngle = Vector3.Angle(endPoint.position, df.transform.forward);

        if (startAngle < endAngle)
        {
            return startPoint;
        }
        else
        {
            return endPoint;
        }
    }

    public Transform getOtherPoint(Transform point)
    {
        if(point.position == startPoint.position)
        {
            return endPoint;
        }
        else
        {
            return startPoint;
        }
    }

    public Transform getPoint(Transform point)
    {
        if (point.position == startPoint.position)
        {
            return startPoint;
        }
        else
        {
            return endPoint;
        }
    }

    public Vector3 getClosestPoint(Vector3 point)
    {
        return ClosestPointOnLine(this.startPoint.position, this.endPoint.position, point);
    }

    public Vector3 getClosestPoint(DeltaFlyer df, AirStream stream)
    {
        return ClosestPointOnLine(stream.startPoint.position, stream.endPoint.position, df.transform.position);
    }

    public float getDistOfClosestPoint(DeltaFlyer df, AirStream stream)
    {
        return Vector3.Distance(getClosestPoint(df, stream), df.transform.position);
    }

    public bool isClostestAirstream(DeltaFlyer df, AirStream CompareAirstream, AirStream CompareToAirstream)
    {
        if (getDistOfClosestPoint(df, CompareAirstream) < getDistOfClosestPoint(df, CompareToAirstream))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    Vector3 ClosestPointOnLine(Vector3 vA, Vector3 vB, Vector3 vPoint)
    {
        var vVector1 = vPoint - vA;
        var vVector2 = (vB - vA).normalized;

        var d = Vector3.Distance(vA, vB);
        var t = Vector3.Dot(vVector2, vVector1);

        if (t <= 0)
        {
            closestPointOnLineTemp = vA;
            return vA;
        }

        if (t >= d)
        {
            closestPointOnLineTemp = vB;
            return vB;
        }

        var vVector3 = vVector2 * t;

        var vClosestPoint = vA + vVector3;
        closestPointOnLineTemp = vClosestPoint;
        return vClosestPoint;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (GameManager.instance.flyers[0] == null ||
            Vector3.Distance(GameManager.instance.flyers[0].transform.position, closestPointOnLineTemp) <= thickness || alwaysDrawGizmos)
        {
            Vector3 tempClosestPointOnLine = startPoint.position;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(tempClosestPointOnLine, thickness);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(tempClosestPointOnLine, thickness * _AlertMultiplyer);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(tempClosestPointOnLine, 0.3f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
           
        }

    }

    //void addForce()
    //{
    //    if (isActive)
    //    {
    //        Collider[] cols = Physics.OverlapSphere(this.transform.position, thickness / 2);
    //        float maxDist = 0;
    //        foreach (Collider c in cols)
    //        {
    //            float dist = Vector3.Distance(c.transform.position, transform.position);
    //            if (dist > maxDist) maxDist = dist;
    //        }
    //        contactPointsHit.Clear();
    //        foreach (Collider c in cols)
    //        {
    //            ContactPoint cp = c.GetComponent<ContactPoint>();
    //            if (cp != null)
    //            {
    //                contactPointsHit.Add(cp);
    //                cp.force = force - (force / maxDist * Vector3.Distance(c.transform.position, transform.position));
    //                if (!knownContactPoints.Contains(cp)) knownContactPoints.Add(cp);
    //            }

    //        }
    //        foreach (ContactPoint cp in knownContactPoints)
    //        {
    //            if (!contactPointsHit.Contains(cp))
    //            {
    //                cp.force = 0;
    //            }
    //        }
    //    }
    //}
}

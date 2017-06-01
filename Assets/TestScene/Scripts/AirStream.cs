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



    public bool alwaysDrawGizmos;

    // Use this for initialization
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        foreach (DeltaFlyer df in GameManager.instance.flyers)
        {
            Vector3 closestPoint = ClosestPointOnLine(startPoint.position, endPoint.position, df.transform.position);
            float dist = Vector3.Distance(closestPoint, df.transform.position);
            if (dist < thickness)
            {
                //deltaflyer is in the airStream
                df.inAirstream(closestPoint);
                if (df.currentAirStream != this)
                {
                    //entering Airstream
                    df.enableSpeed(getMovingToPoint(df));
                    df.currentAirStream = this;
                }
            }
            else if (dist < (thickness * _AlertMultiplyer) && !df.isInAirstream)
            {
                if (df.detectedAirStream != this)
                {
                    //entering DetectionRange   
                }
                df.detectAirstream(this, closestPoint);

                //Deltaflyer is near the airStream
            }
            else if (df.currentAirStream == this && dist > thickness)
            {
                //leaving AirStream
                df.currentAirStream = null;
                df.disableSpeed();
            }
            else if (df.detectedAirStream == this && dist > (thickness * _AlertMultiplyer))
            {
                //leaving Detection Range
                df.detectedAirStream = null;
                df.resetMotors();
            }

        }
    }

    Transform getMovingToPoint(DeltaFlyer df)
    {
        float startAngle = Vector3.Angle(startPoint.position, df.transform.forward);
        float endAngle = Vector3.Angle(endPoint.position, df.transform.forward);

        if (startAngle < endAngle)
        {
            Debug.Log("going to startpoint");
            return startPoint;
        }
        else
        {
            Debug.Log("going to endpoint");
            return endPoint;
        }
    }

    public Vector3 getClosestPoint(DeltaFlyer df)
    {
        return ClosestPointOnLine(startPoint.position, endPoint.position, df.transform.position);
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
        if (GameManager.instance.flyers[0] == null ||
            Vector3.Distance(GameManager.instance.flyers[0].transform.position, closestPointOnLineTemp) <= thickness || alwaysDrawGizmos)
        {

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(closestPointOnLineTemp, thickness);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(closestPointOnLineTemp, thickness * _AlertMultiplyer);
            Gizmos.color = Color.red;
            if (closestPointOnLineTemp != Vector3.zero)
                Gizmos.DrawSphere(closestPointOnLineTemp, 0.3f);
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

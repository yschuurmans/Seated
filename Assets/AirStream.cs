using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStream : MonoBehaviour
{
    private const int _NotifyMultiplyer = 3;
    public float thickness;
    public Transform startPoint;
    public Transform endPoint;

    public static AirStream Instance;

    public float force;
    public float radius;
    private List<ContactPoint> contactPointsHit = new List<ContactPoint>();
    private List<ContactPoint> knownContactPoints = new List<ContactPoint>();

    // Use this for initialization
    void Start()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        foreach (DeltaFlyer df in GameManager.instance.flyers)
        {
            float dist = Vector3.Distance(ClosestPointOnLine(startPoint.position, endPoint.position, df.transform.position), df.transform.position);
            if (dist < thickness)
            {
                //deltaflyer is in the airStream
                addForce(df);
            }
            else if (dist < (thickness * _NotifyMultiplyer))
            {
                //Deltaflyer is near the airStream
                noticePlayer(df);
            }
        }
    }

    void noticePlayer(DeltaFlyer df)
    {

        ContactPoint[] points = df.contactPoints;
        ContactPoint closestPoint = points[0];
        float minDist = 0;
        foreach (ContactPoint c in points)
        {
            float dist = Vector3.Distance(c.transform.position, transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestPoint = c;
            }
        }

        closestPoint.force = (force / minDist * Vector3.Distance(closestPoint.transform.position, transform.position));
    }

    void addForce(DeltaFlyer df)
    {
        ContactPoint[] points = df.contactPoints;
        float maxDist = 0;
        foreach (ContactPoint c in points)
        {
            float dist = Vector3.Distance(c.transform.position, transform.position);
            if (dist > maxDist) maxDist = dist;
        }

        contactPointsHit.Clear();
        foreach (ContactPoint c in points)
        {
            contactPointsHit.Add(c);
            c.force = force - (force / maxDist * Vector3.Distance(c.transform.position, transform.position));
            if (!knownContactPoints.Contains(c)) knownContactPoints.Add(c);
        }

        foreach (ContactPoint cp in knownContactPoints)
        {
            if (!contactPointsHit.Contains(cp))
            {
                cp.force = 0;
            }
        }
    }


    Vector3 ClosestPointOnLine(Vector3 vA, Vector3 vB, Vector3 vPoint)
    {
        var vVector1 = vPoint - vA;
        var vVector2 = (vB - vA).normalized;

        var d = Vector3.Distance(vA, vB);
        var t = Vector3.Dot(vVector2, vVector1);

        if (t <= 0)
            return vA;

        if (t >= d)
            return vB;

        var vVector3 = vVector2 * t;

        var vClosestPoint = vA + vVector3;

        return vClosestPoint;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(startPoint.position, thickness);
        Gizmos.DrawWireSphere(endPoint.position, thickness);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPoint.position, thickness * _NotifyMultiplyer);
        Gizmos.DrawWireSphere(endPoint.position, thickness * _NotifyMultiplyer);
        //foreach (var cp in contactPointsHit)
        //{
        //    if (cp != null)
        //    {
        //        Gizmos.color = Color.green;
        //        Gizmos.DrawSphere(cp.transform.position, 0.2f);
        //    }
        //}
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

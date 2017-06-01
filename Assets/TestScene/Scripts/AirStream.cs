using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AirStream : MonoBehaviour
{
    private const int _AlertMultiplyer = 3;
    public float thickness;
    public Transform startPoint;
    public Transform endPoint;


    public const float _MaxAlertForcePerc = 40;
    public const float _MinAlertForcePerc = 5;
    public Vector3 closestPointOnLineTemp;
    public float force;
    private List<ContactPoint> contactPointsHit = new List<ContactPoint>();
    private List<ContactPoint> knownContactPoints = new List<ContactPoint>();


    private float minImpactRadius = 1f;
    private float maxImpactRadius = 1.6f;

    //gizmos variables
    public bool alwaysDrawGizmos;
    public float impactRadius;
    Vector3 gizClosesImpactPoint;

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
                addForce(df, closestPoint);
                if (df.currentAirStream != this)
                {
                    df.currentAirStream = this;
                }
            }
            else if (dist < (thickness * _AlertMultiplyer) && !df.inAirstream)
            {
                //Deltaflyer is near the airStream
                alertPlayer(df, closestPoint);
            }
            else if (df.currentAirStream == this && dist > thickness)
            {
                df.currentAirStream = null;
            }

        }
    }


    void alertPlayer(DeltaFlyer df, Vector3 closestPointOnLine)
    {
        //Vector3 closestImpactPoint = df.raptor.raptorCollider.ClosestPointOnBounds(closestPointOnLine);
        RaycastHit hit;
        //Vector3 heading = closestPointOnLine - df.raptor.raptorCollider.center;
        //float distance = heading.magnitude;
        Vector3 direction = (df.raptor.raptorCollider.transform.position - closestPointOnLine).normalized;
        Ray ray = new Ray(closestPointOnLine, direction);
        Physics.Raycast(ray, out hit);
        Vector3 closestImpactPoint = hit.point;
        gizClosesImpactPoint = closestImpactPoint;

        float dist = Vector3.Distance(closestPointOnLine, closestImpactPoint) - thickness;
        float range = thickness * _AlertMultiplyer - thickness;
        float distPerc = (100 - (dist / (range / 100))) / 100;

        //ContactPoint closestPoint = df.contactPoints.OrderBy(p => Vector3.Distance(p.transform.position, closestPointOnLine)).FirstOrDefault();
        //ContactPoint furthestPoint = df.contactPoints.OrderByDescending(p => Vector3.Distance(p.transform.position, closestPointOnLine)).FirstOrDefault();

        //float minRadius = Vector3.Distance(closestImpactPoint, closestPoint.transform.position);
        //float maxRadius = Vector3.Distance(closestImpactPoint, furthestPoint.transform.position);
        //float radiusRange = (maxRadius - minRadius) + minRadius;
        float radiusRange = maxImpactRadius - minImpactRadius;

        float radius = (radiusRange * distPerc) + minImpactRadius;
        impactRadius = radius;


        List<ContactPoint> points = df.contactPoints.Where(p => Vector3.Distance(p.transform.position, closestImpactPoint) <= radius).ToList();

        foreach (ContactPoint c in points)
        {

            //float impactDist = Vector3.Distance(c.transform.position, closestImpactPoint);
            //float impactRange = radius;
            //float impactDistPerc = (100 - (impactDist / (impactRange / 100))) / 100;
            //c.force = force * impactDistPerc;
            c.force = getForce(c.transform.position, closestImpactPoint, _MinAlertForcePerc, radius);
        }








        //ContactPoint[] points = df.contactPoints;
        //ContactPoint closestPoint = points.OrderBy(p => Vector3.Distance(p.transform.position, closestPointOnLine)).FirstOrDefault();

        //if (!knownContactPoints.Contains(closestPoint)) knownContactPoints.Add(closestPoint);

        ////foreach (ContactPoint c in points)
        ////{
        ////    float dist = Vector3.Distance(c.transform.position, transform.position);
        ////    if (dist < minDist)
        ////    {
        ////        minDist = dist;
        ////        closestPoint = c;
        ////    }
        ////}

        ////|*******|--------------------*--------)

        //float dist = Vector3.Distance(closestPoint.transform.position, closestPointOnLine) - thickness;
        //float range = thickness * _AlertMultiplyer - thickness;
        //float perc = 100 - (dist / (range / 100));
        //float maxForce = force * ((_MaxAlertForcePerc - _MinAlertForcePerc) / 100);
        //float beginForce = force * (_MinAlertForcePerc / 100);

        //closestPoint.force = maxForce * (perc / 100) + beginForce;

        //foreach (ContactPoint cp in knownContactPoints)
        //{
        //    if (cp != closestPoint)
        //    {
        //        cp.force = 0;
        //    }
        //}


        //closestPoint.force = (force / (thickness * _AlertMultiplyer * Vector3.Distance(closestPoint.transform.position, closestPointOnLine)));
    }

    public float getForce(Vector3 objectPos, Vector3 closestPointOnLine, float minForcePerc, float range)
    {
        float dist = Vector3.Distance(objectPos, closestPointOnLine);
        float perc = (100 - (dist / (range / 100))) / 100;

        float minForce = force * (minForcePerc / 100);
        float tempForce = force - minForce;
        float newForce = tempForce * perc;
        float totalForce = newForce + minForce;

        return totalForce;
    }


    void addForce(DeltaFlyer df, Vector3 closestPoint)
    {
        ContactPoint[] points = df.contactPoints;
        float maxDist = 0;
        foreach (ContactPoint c in points)
        {
            float dist = Vector3.Distance(c.transform.position, closestPoint);
            if (dist > maxDist) maxDist = dist;
        }

        contactPointsHit.Clear();
        foreach (ContactPoint c in points)
        {
            contactPointsHit.Add(c);




            float range = thickness;
            float dist = range - Vector3.Distance(c.transform.position, closestPoint);
            if (dist < 0) continue;
            float perc = (dist / (range / 100));
            //|****-**|----------------------------)
            //range = 20
            //dist = 5
            //perc = 25%
            //in 100% you have to deal 70% of the force + 30%
            //you are in 25% so you have to deal 70/4 of the force + 30%

            //the force + extra force (so motors have a higher average force
            float tempTotalForce = force + force * (_MaxAlertForcePerc / 100);
            float tempForce = (tempTotalForce - (tempTotalForce / maxDist * Vector3.Distance(c.transform.position, closestPoint))) * 10;
            //the max force the motors will have, this is the total force - the max force of the alert section
            float maxForce = tempForce * ((100 - _MaxAlertForcePerc) / 100);

            //extra force to make sure the transistion of the alert section and the inner section is going fluidly
            float extraForce = tempForce * (_MaxAlertForcePerc / 100);

            //the force of this section, no extra force added
            float ringForce = maxForce * (perc / 100);
            //the totalforce calculated
            float totalForce = ringForce + extraForce;
            c.force = totalForce;

            //c.force = force - (force / maxDist * Vector3.Distance(c.transform.position, closestPoint));

            //c.force = totalForce;



            //c.force = (((perc/100) * (100 - _MaxAlertForcePerc) / 100) * force) + ((_MaxAlertForcePerc / 100) * force);
            // c.force = force - (force / maxDist * Vector3.Distance(c.transform.position, closestPoint));
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
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(gizClosesImpactPoint, impactRadius);
        }
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

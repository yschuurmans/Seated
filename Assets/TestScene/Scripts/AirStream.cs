using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AirStream : MonoBehaviour
{
    private const int _AlertMultiplyer = 7;
    public float thickness;
    public Transform startPoint;
    public Transform endPoint;
    

    public const float _MaxAlertForcePerc = 40;
    public const float _MinAlertForcePerc = 20;
    public Vector3 closestPointOnLineTemp;
    public float force;
    private List<ContactPoint> contactPointsHit = new List<ContactPoint>();
    private List<ContactPoint> knownContactPoints = new List<ContactPoint>();

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
            }
            else if (dist < (thickness * _AlertMultiplyer))
            {
                //Deltaflyer is near the airStream
                alertPlayer(df, closestPoint);
            }
        }
    }
   

    void alertPlayer(DeltaFlyer df, Vector3 closestPointOnLine)
    {

        ContactPoint[] points = df.contactPoints;
        ContactPoint closestPoint = points.OrderBy(p => Vector3.Distance(p.transform.position, closestPointOnLine)).FirstOrDefault();

        if (!knownContactPoints.Contains(closestPoint)) knownContactPoints.Add(closestPoint);

        //foreach (ContactPoint c in points)
        //{
        //    float dist = Vector3.Distance(c.transform.position, transform.position);
        //    if (dist < minDist)
        //    {
        //        minDist = dist;
        //        closestPoint = c;
        //    }
        //}

        //|*******|--------------------*--------)

        float dist = Vector3.Distance(closestPoint.transform.position, closestPointOnLine) - thickness;
        float range = thickness * _AlertMultiplyer - thickness;
        float perc = 100 - (dist / (range / 100));
        float maxForce = force * ((_MaxAlertForcePerc - _MinAlertForcePerc) / 100);
        float beginForce = force * (_MinAlertForcePerc / 100);

        closestPoint.force = maxForce * (perc / 100) + beginForce;

        foreach (ContactPoint cp in knownContactPoints)
        {
            if (cp != closestPoint)
            {
                cp.force = 0;
            }
        }

        //closestPoint.force = (force / (thickness * _AlertMultiplyer * Vector3.Distance(closestPoint.transform.position, closestPointOnLine)));
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

            float tempForce = (force - (force / maxDist * Vector3.Distance(c.transform.position, closestPoint))) * 10;
            float maxForce = tempForce * ((100 - _MaxAlertForcePerc) / 100);
            float extraForce = tempForce * (_MaxAlertForcePerc / 100);

            float ringForce = maxForce * (perc / 100);
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(closestPointOnLineTemp, thickness);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(closestPointOnLineTemp, thickness * _AlertMultiplyer);
        Gizmos.color = Color.red;
        if (closestPointOnLineTemp != Vector3.zero)
            Gizmos.DrawSphere(closestPointOnLineTemp, 0.3f);
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

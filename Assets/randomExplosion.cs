using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomExplosion : MonoBehaviour {

    public float force;
    public float radius;
    public bool doExplosion;
    private List<ContactPoint> contactPointsHit = new List<ContactPoint>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (doExplosion)
        {
            Collider[] cols = Physics.OverlapSphere(this.transform.position, radius);
            float maxDist = 0;
            foreach(Collider c in cols)
            {
                float dist = Vector3.Distance(c.transform.position, transform.position);
                if (dist > maxDist) maxDist = dist;
            }
            contactPointsHit.Clear();
            foreach(Collider c in cols)
            {
                ContactPoint cp = c.GetComponent<ContactPoint>();
                contactPointsHit.Add(cp);
                if (cp != null)
                {
                    cp.force = force - (force / maxDist * Vector3.Distance(c.transform.position, transform.position));
                }
            }
        }
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
        foreach (var cp in contactPointsHit)
        {
            if (cp != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(cp.transform.position, 0.2f);
            }
        }
    }
}

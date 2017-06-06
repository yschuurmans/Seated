using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoSphere : MonoBehaviour
{

    public Color GizmoColor = Color.red;
    public float Size = 0.2f;

    void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        Gizmos.DrawSphere(transform.position, Size);
    }
}

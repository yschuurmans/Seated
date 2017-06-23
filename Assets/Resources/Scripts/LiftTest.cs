using UnityEngine;

public class LiftTest : MonoBehaviour
{
    private int maxVelocity = 100;
    private float velocity;

    private Transform tf;
    private Rigidbody rb;
    
    void Awake()
    {
        tf = transform;
        rb = GetComponent<Rigidbody>();
        velocity = 0;
    }

    void update()
    {
        
    }
}
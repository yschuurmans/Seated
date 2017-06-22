using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript instance;
    Camera cam;
    public bool doShake;
    float shake;
    public float normalShake;
    public float airStreamShake;
    public float decreaseFactor;
    Vector3 origPos;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        cam = GetComponent<Camera>();
    }

    // Use this for initialization
    void Start()
    {
        origPos = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Shake();
    }

    void Shake()
    {
        //if (doShake)
        //{
        //    cam.transform.localPosition = Random.insideUnitSphere * shakeAmount;
        //}

        if (doShake)
        {
            cam.transform.localPosition = (Random.insideUnitSphere * shake) + origPos;
        }
    }

    public void EneteredAirstream()
    {
        shake = airStreamShake;
    }

    public void LeftAirstream()
    {
        shake = normalShake;
    }
}

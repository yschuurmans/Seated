using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public RaptorManager rm;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startWave()
    {
        rm.btnStartWave();
    }

    public void startHeartbeat()
    {
        rm.btnStartHeartbeat();
    }

    public void toggleAllRaptors()
    {
        rm.btnToggleAllRaptors();
    }

    public void randomJolt()
    {
        rm.randomJolt();
    }
}

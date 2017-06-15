using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirstreamManager : MonoBehaviour {


    public ParticleSystem ps;
    List<ParticleSystem> psPool = new List<ParticleSystem>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private ParticleSystem getParticleSystem()
    {
        foreach (ParticleSystem sys in psPool)
        {
            if (!sys.gameObject.activeSelf)
            {
                sys.gameObject.SetActive(true);
                return sys;
            }
        }

        ParticleSystem newSys = Instantiate<ParticleSystem>(ps);
        psPool.Add(newSys);
        newSys.gameObject.SetActive(true);
        return newSys;
    }
}

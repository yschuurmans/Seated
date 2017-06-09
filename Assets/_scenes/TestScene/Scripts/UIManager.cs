using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject FollowObject;
    public GameObject objToFollow;
    Rect screenRect = new Rect(0, 0, Screen.width - 10, Screen.height - 10);

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(objToFollow.transform.position + new Vector3(0, 4, 0));

        if (screenRect.Contains(pos))
        {
            Bounds bounds = new Bounds(screenRect.center, screenRect.size);
            // Inside
            FollowObject.transform.position = pos;
        }
        else
        {
            //screenRect.b
        }

    }
}

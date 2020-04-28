using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    void Start()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    void onTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "walker")
        {
            collider.GetComponent<AnimationAgent>().TouchedTarget();
        }
    }
}

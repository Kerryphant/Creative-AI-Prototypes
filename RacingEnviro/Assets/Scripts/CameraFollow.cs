using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public CarAgent[] targets;
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.3f;
    private int frames = 0;

    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        if (frames >= 500)
        {
            //Select target
            CarAgent highestVal = targets[0];
            
            foreach (CarAgent agent in targets)
            {
                if (highestVal.GetCumulativeReward() < agent.GetCumulativeReward())
                {
                    highestVal = agent;
                }
            }

            target = highestVal.transform.GetChild(3).transform;
        } else
        {
            frames++;
        }
    }

    private void FixedUpdate()
    {
        Vector3 targetPos = target.TransformPoint(offset);
        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothSpeed);

        transform.position = smoothedPos;

        transform.LookAt(target);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.3f;

    private Vector3 velocity = Vector3.zero;

    private void FixedUpdate()
    {
        Vector3 targetPos = target.TransformPoint(offset);
        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothSpeed);

        transform.position = smoothedPos;

        transform.LookAt(target);
    }
}

using System;
using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public class Checkpoint : MonoBehaviour
{
    public event Action<CarController, Checkpoint> OnReachCheckpoint;

    public bool isFinishLine;
    public LayerMask racerLayers;

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    //When a car enters the collider, invoke the reached checkpoint event
    private void OnTriggerEnter(Collider other)
    {
        CarController car = other.GetComponentInParent<CarController>();

        if (car != null)
            OnReachCheckpoint.Invoke(car, this);
    }
}

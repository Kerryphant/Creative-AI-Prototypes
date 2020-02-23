using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RaceManager : MonoBehaviour
{
    public int lapCount;
    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    [System.Serializable]
    public class CheckPointEvent : UnityEvent<Checkpoint> { }

    public CheckPointEvent reachedCorrectCheckpoint;

    Dictionary<GameObject, Checkpoint> nextCheckpoint = new Dictionary<GameObject, Checkpoint>(16);

    void OnEnable()
    {
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].OnReachCheckpoint += CheckHitCheckpoint;
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].OnReachCheckpoint -= CheckHitCheckpoint;
        }
    }

    private void Start()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");

        for (int i = 0; i < cars.Length; i++)
        {
            nextCheckpoint.Add(cars[i], checkpoints[0]);
        }
    }

    void CheckHitCheckpoint(CarController car, Checkpoint checkpoint)
    {
        if (ContainsKeyPair(car.gameObject, checkpoint))
        {
            nextCheckpoint[car.gameObject] = checkpoints[GetNextCheckpoint(checkpoints.IndexOf(checkpoint))];
            HitCorrectCheckpoint(car, checkpoint);
        }
    }

    void HitCorrectCheckpoint(CarController car, Checkpoint checkpoint)
    {
        reachedCorrectCheckpoint.Invoke(checkpoint);
    }

    public bool ContainsKeyPair(GameObject key, Checkpoint value)
    {
        return nextCheckpoint.TryGetValue(key, out Checkpoint outValue) && value.Equals(outValue);
    }

    int GetNextCheckpoint(int curIndex)
    {
        int index = (curIndex + 1) % checkpoints.Count;
        return index;
    }
}
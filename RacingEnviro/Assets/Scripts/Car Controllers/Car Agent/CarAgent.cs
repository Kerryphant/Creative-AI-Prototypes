using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using TMPro;

public class CarAgent : Agent
{
    public RaceManager raceManager;
    CarController controller;

    float horizontal;
    float accelerate;
    float brake;
    bool handbrake = false;

    Vector3 startPos;
    Quaternion startRot;

    public float checkpointReward = 1;

    public TextMeshPro rewardText;

    private void Start()
    {
        controller = GetComponent<CarController>();

        startPos = this.transform.position;
        startRot = this.transform.rotation;
    }

    public override void AgentReset()
    {
        base.AgentReset();

        transform.position = startPos;
        transform.rotation = startRot;
        controller.Reset();
    }

    public void ReachedCheckpoint(Checkpoint checkpoint)
    {
        AddReward(checkpointReward);

        Debug.Log("Reached Checkpoint");
    }

    public override void AgentAction(float[] vectorAction)
    {
        base.AgentAction(vectorAction);

        accelerate = vectorAction[0];

        horizontal = vectorAction[1];

        brake = vectorAction[2];

        AddReward(controller.CurrentSpeed * .001f);
    }

    private void FixedUpdate()
    {
        if (GetStepCount() % 5 == 0)
        {
            RequestDecision();
        }
        else
        {
            RequestAction();
        }

        controller.Move(horizontal, accelerate, brake, handbrake);

        rewardText.text = GetCumulativeReward().ToString("0.00");

        if(GetCumulativeReward() < -50)
        {
            Done();
        }

        //Debug.Log(GetCumulativeReward());
    }

    //Collecting all the non raycast observations
    public override void CollectObservations()
    {
        //Current speed of the car (1 float = 1 value)
        AddVectorObs(controller.CurrentSpeed);

        //Direction the car is going (1 vector3 = 3 values)
        AddVectorObs(transform.forward);
    }
}

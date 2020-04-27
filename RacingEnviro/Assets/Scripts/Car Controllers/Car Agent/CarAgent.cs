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
    float samePosTime;
    bool handbrake = false;

    Vector3 startPos;
    Vector3 lastPos;
    Quaternion startRot;

    public float checkpointReward = 1;

    public TextMeshPro rewardText;

    private void Start()
    {
        controller = GetComponent<CarController>();

        startPos = this.transform.position;
        startRot = this.transform.rotation;
    }

    //Reset the agent to it's starting position
    public override void AgentReset()
    {
        base.AgentReset();

        transform.position = startPos;
        transform.rotation = startRot;
        controller.Reset();
        raceManager.ResetAgentCP(this.gameObject);
    }

    //Give the agent a reward when it reaches a checkpoint
    public void ReachedCheckpoint(Checkpoint checkpoint)
    {
        AddReward(checkpointReward);
    }

    //Sets all the agent actions;
    public override void AgentAction(float[] vectorAction)
    {
        base.AgentAction(vectorAction);

        accelerate = vectorAction[0];

        horizontal = vectorAction[1];

        brake = vectorAction[2];
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

        //Reward the agent based on it's current speed, making the incentive getting around the track quickly
        AddReward(controller.CurrentSpeed * .001f);

        //If the agent somehow manages to get to -50 reward reset it
        if (GetCumulativeReward() < -50)
        {
            Done();
        }

        //Punish the agent if it isn't moving
        if(controller.CurrentSpeed == 0)
        {
            AddReward(-0.1f);
        }

        if (lastPos == gameObject.transform.position)
        {
            samePosTime += Time.deltaTime;

            if(samePosTime >= 5)
            {
                Done();
            }
        }

        lastPos = gameObject.transform.position;
    }

    //Collecting all the non raycast observations
    public override void CollectObservations()
    {
        //Current speed of the car (1 float = 1 value)
        AddVectorObs(controller.CurrentSpeed);

        //Direction the car is going (1 vector3 = 3 values)
        AddVectorObs(transform.forward);

        //Position of the car (1 vector 3 = 3 values)
        AddVectorObs(transform.position);

        //Position of next checkpoint (1 vector 3 = 3 values)
        AddVectorObs(raceManager.nextCheckpoint[this.gameObject].transform.position);
    }
}

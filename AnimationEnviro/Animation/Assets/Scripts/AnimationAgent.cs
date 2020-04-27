using UnityEngine;
using MLAgents;
using MLAgentsExamples;

using System.Collections;

public class AnimationAgent : Agent
{
    private float randomNum = 0;
    private float distReward;

    public Transform target;

    public Vector3 startPos;
    public Quaternion startRot;

    public Transform body;

    public Transform[] upperJoints = new Transform[8];
    public Transform[] lowerJoints = new Transform[8];

    public Rigidbody bodyrb;

    Vector3 targetY;
    Vector3 dirToTarget;

    JointDriveController jointControl;

    public override void InitializeAgent()
    {
        base.InitializeAgent();

        bodyrb = body.GetComponent<Rigidbody>();

        startPos = transform.position;
        startRot = transform.rotation;

        jointControl = GetComponent<JointDriveController>();

        //Setup the body parts in the joint controller
        jointControl.SetupBodyPart(body);

        for (int i = 0; i < 8; i++)
        {
            jointControl.SetupBodyPart(upperJoints[i]);
        }

        for (int i = 0; i < 8; i++)
        {
            jointControl.SetupBodyPart(lowerJoints[i]);
        }
    }

    public void CollectObservationsBody(BodyPart bodyPart)
    {
        var rigidBody = bodyPart.rb;

        //Give each body part's position relative to the walker's body
        //Give each body part's rotation
        //Give each body part's strength
        if (bodyPart.rb.transform != body)
        {
            var localPosRelToBody = body.InverseTransformPoint(rigidBody.position);
            AddVectorObs(localPosRelToBody);
            AddVectorObs(bodyPart.currentXNormalizedRot); // Current x rot
            AddVectorObs(bodyPart.currentYNormalizedRot); // Current y rot
            AddVectorObs(bodyPart.currentZNormalizedRot); // Current z rot
            AddVectorObs(bodyPart.currentStrength / jointControl.maxJointForceLimit);
        }
    }

    public override void CollectObservations()
    {
        jointControl.GetCurrentJointForces();

        //Give direction and distance to target
        var targetPos = target.position;
        targetPos.y = body.position.y;
        dirToTarget = targetPos - body.position;
        dirToTarget.Normalize();

        float distToTarget = Vector3.Distance(body.position, target.position);

        AddVectorObs(distToTarget);
        AddVectorObs(dirToTarget);

        //Give forward and up of body
        AddVectorObs(body.forward);
        AddVectorObs(body.up);

        //Give dot product of body forward and target direction
        AddVectorObs(Vector3.Dot(dirToTarget, body.forward));

        //Give information about each body part
        foreach(var bodyPart in jointControl.bodyPartsDict.Values)
        {
            CollectObservationsBody(bodyPart);
        }
    }

    public override void AgentAction(float[] vectorAction)
    {
        // The dictionary with all the body parts in it are in the jdController
        var bodyDictionary = jointControl.bodyPartsDict;

        var i = -1;
        // Pick a new target joint rotation
        for (int j = 0; j < 8; j++)
        {
            bodyDictionary[upperJoints[j]].SetJointTargetRotation(0, vectorAction[++i], vectorAction[++i]);

            bodyDictionary[upperJoints[j]].SetJointStrength(vectorAction[++i]);
        }

        for (int j = 0; j < 8; j++)
        {
            bodyDictionary[lowerJoints[j]].SetJointTargetRotation(0, 0, vectorAction[++i]);

            bodyDictionary[lowerJoints[j]].SetJointStrength(vectorAction[++i]);
        }
    }

    public override float[] Heuristic()
    {
        float[] action = new float[40];

        for(int i = 0; i < 40; i++)
        {
            randomNum = Random.Range(-1f, 1f);
            action[i] = randomNum;
        }

        return action;
    }

    private void FixedUpdate()
    {
        //Direction to target
        //We don't care about the y coordinate of the target
        //as the agent is only moving on the xz plane
        //so we'll set the targets y position to the body's y position
        var targetPos = target.position;
        targetPos.y = body.position.y;
        dirToTarget = targetPos - body.position;
        dirToTarget.Normalize();

        //Give positive reward if angle between walker's forward and the direction 
        //to the target is less than 90 degrees
        //else give a negative reward
        float targetBodyDot = Vector3.Dot(dirToTarget, body.forward);
        AddReward(1f / (targetBodyDot * 1000));

        //Give reward based on the distance of the agent from the target
        float distToTarget = Vector3.Distance(body.position, target.position);
        distReward = distToTarget > 75 ? -1 : 1;

        AddReward(1f / (distReward * distToTarget * 10));

        //Give negative reward every step
        if (maxStep > 0)
        {
            AddReward(-1f / maxStep);
        }

        if (GetStepCount() % 5 == 0)
        {
            RequestDecision();
        }
        else
        {
            RequestAction();
        }
    }

    public override void AgentReset()
    {
        foreach (var bodyPart in jointControl.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }

        Respawn();
    }

    public void TouchedTarget()
    {
        AddReward(1f);
        Respawn();
    }

    public void Respawn()
    {
        transform.position = startPos;
        transform.rotation = startRot;
    }
}

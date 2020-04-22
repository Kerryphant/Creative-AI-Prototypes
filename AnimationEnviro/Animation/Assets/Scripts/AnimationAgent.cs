using UnityEngine;
using MLAgents;
using MLAgentsExamples;

public class AnimationAgent : Agent
{
    public Transform target;

    public Vector3 startPos;
    public Quaternion startRot;

    public Transform body;
    //public Transform[] joints = new Transform[16];
    public Transform jointUpper1;
    public Transform jointLower1;
    public Transform jointUpper2;
    public Transform jointLower2;
    public Transform jointUpper3;
    public Transform jointLower3;
    public Transform jointUpper4;
    public Transform jointLower4;
    public Transform jointUpper5;
    public Transform jointLower5;
    public Transform jointUpper6;
    public Transform jointLower6;
    public Transform jointUpper7;
    public Transform jointLower7;
    public Transform jointUpper8;
    public Transform jointLower8;

    int MaxStep = 5000;

    Vector3 targetY;
    Vector3 dirToTarget;

    JointDriveController jointControl;

    public override void InitializeAgent()
    {
        base.InitializeAgent();

        startPos = transform.position;
        startRot = transform.rotation;

        jointControl = GetComponent<JointDriveController>();

        //Setup the body parts in the joint controller
        jointControl.SetupBodyPart(body);

        /*foreach (var joint in joints)
        {
            jointControl.SetupBodyPart(joint);
        }*/

        jointControl.SetupBodyPart(jointUpper1);
        jointControl.SetupBodyPart(jointUpper2);
        jointControl.SetupBodyPart(jointUpper3);
        jointControl.SetupBodyPart(jointUpper4);
        jointControl.SetupBodyPart(jointUpper5);
        jointControl.SetupBodyPart(jointUpper6);
        jointControl.SetupBodyPart(jointUpper7);
        jointControl.SetupBodyPart(jointUpper8);
        jointControl.SetupBodyPart(jointLower1);
        jointControl.SetupBodyPart(jointLower2);
        jointControl.SetupBodyPart(jointLower3);
        jointControl.SetupBodyPart(jointLower4);
        jointControl.SetupBodyPart(jointLower5);
        jointControl.SetupBodyPart(jointLower6);
        jointControl.SetupBodyPart(jointLower7);
        jointControl.SetupBodyPart(jointLower8);
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

        dirToTarget = target.position - body.position;
        targetY = new Vector3(0f, dirToTarget.y, 0f);
        dirToTarget = dirToTarget - targetY;
        dirToTarget.Normalize();

        float distToTarget = Vector3.Distance(body.position, target.position);

        AddVectorObs(distToTarget);
        AddVectorObs(dirToTarget);

        //Give forward and up of body
        AddVectorObs(body.forward);
        AddVectorObs(body.up);

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
        bodyDictionary[jointUpper1].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bodyDictionary[jointUpper2].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bodyDictionary[jointUpper3].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bodyDictionary[jointUpper4].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bodyDictionary[jointUpper5].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bodyDictionary[jointUpper6].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bodyDictionary[jointUpper7].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bodyDictionary[jointUpper8].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);

        bodyDictionary[jointLower1].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bodyDictionary[jointLower2].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bodyDictionary[jointLower3].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bodyDictionary[jointLower4].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bodyDictionary[jointLower5].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bodyDictionary[jointLower6].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bodyDictionary[jointLower7].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bodyDictionary[jointLower8].SetJointTargetRotation(vectorAction[++i], 0, 0);

        bodyDictionary[jointUpper1].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointUpper2].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointUpper3].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointUpper4].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointUpper5].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointUpper6].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointUpper7].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointUpper8].SetJointStrength(vectorAction[++i]);

        bodyDictionary[jointLower1].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointLower2].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointLower3].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointLower4].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointLower5].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointLower6].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointLower7].SetJointStrength(vectorAction[++i]);
        bodyDictionary[jointLower8].SetJointStrength(vectorAction[++i]);
    }

    void FixedUpdate()
    {
        if (GetStepCount() % 5 == 0)
        {
            RequestDecision();
        }
        else
        {
            RequestAction();
        }

        //Direction to target
        dirToTarget = target.position - body.position;
        dirToTarget.Normalize();

        //Give positive reward if angle between walker's forward and the direction 
        //to the target is less than 90 degrees
        //else give a negative reward
        float targetBodyDot = Vector3.Dot(dirToTarget, body.forward);
        AddReward(1f / (targetBodyDot * 1000));

        //Give small reward that increases as the walker gets closer to the target
        float distToTarget = Vector3.Distance(body.position, target.position);
        if (distToTarget < 50)
        {
            AddReward(1f / (distToTarget * 100));
        }

        //Give negative reward every step
        AddReward(-1f / MaxStep);
    }

    /*public override float[] Heuristic()
    {

    }*/

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

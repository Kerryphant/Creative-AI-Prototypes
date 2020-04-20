using UnityEngine;
using MLAgents;
using MLAgentsExamples;

public class AnimationAgent : Agent
{
    public Transform target;

    public Vector3 startPos;
    public Quaternion startRot;

    public Transform body;
    public Transform[] joints = new Transform[16];

    JointDriveController jointControl;

    public override void InitializeAgent()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        jointControl = GetComponent<JointDriveController>();

        //Setup the body parts in the joint controller
        jointControl.SetupBodyPart(body);

        foreach (var joint in joints)
        {
            jointControl.SetupBodyPart(joint);
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
        Vector3 targetY;

        Vector3 dirToTarget = target.position - body.position;
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
        for (int j = 0; j < 16; j += 2)
        {
            bodyDictionary[joints[j]].SetJointTargetRotation(vectorAction[i++], vectorAction[i++], 0);
            bodyDictionary[joints[j]].SetJointStrength(vectorAction[i++]);

            bodyDictionary[joints[j+1]].SetJointTargetRotation(vectorAction[i++], 0, 0);
            bodyDictionary[joints[j+1]].SetJointStrength(vectorAction[i++]);
        }
    }

    void FixedUpdate()
    {

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

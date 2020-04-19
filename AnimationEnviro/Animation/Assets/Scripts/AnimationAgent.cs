using UnityEngine;
using MLAgents;
using MLAgentsExamples;

public class AnimationAgent : Agent
{
    public Vector3 startPos;

    public override void InitializeAgent()
    {
        startPos = transform.position;
    }

    public override void CollectObservations()
    {

    }

    public override void AgentAction(float[] vectorAction)
    {

    }

    void FixedUpdate()
    {

    }

    public override void AgentReset()
    {
        Respawn();
    }

    public void Respawn()
    {
        transform.position = startPos;
    }
}

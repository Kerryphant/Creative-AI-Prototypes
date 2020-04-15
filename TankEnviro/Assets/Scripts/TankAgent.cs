using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


public class TankAgent : Agent
{
	//area the tank is training in
	private TankArea tankArea;

	//tank's rigid body
	Rigidbody rigidbody;

	//tank acceleration
	private const float acceleration = 0.5f;
	//tank turn speed
	private const float turnStrength = 1;
    /// The Wwise event to trigger the tank engine sound
	public AK.Wwise.Event TankTreads = new AK.Wwise.Event();


    private void Start()
	{
		tankArea = GetComponentInParent<TankArea>();
		rigidbody = GetComponent<Rigidbody>();
	}

	public override void AgentAction(float[] vectorAction)
	{
		//convert actions to axis values
		//MOVE FORWARD/BACKWARD - this needs to be changed as the tank slides when it shouldn't
		if (vectorAction[0] == 1f)
		{
			//move forwards
			rigidbody.AddForce(transform.forward * acceleration, ForceMode.VelocityChange);
            //Plays TankTreads Event
            TankTreads.Post(gameObject);
        }
		else if (vectorAction[0] == 2f)
		{
			//move backwards
			rigidbody.AddForce(-transform.forward * acceleration, ForceMode.VelocityChange);
		}

		//MOVE LEFT/RIGHT
		if (vectorAction[1] == 1f)
		{
			//turn left by rotating around y axis
			transform.Rotate(0.0f, -turnStrength, 0.0f, Space.Self);
		}
		else if (vectorAction[1] == 2f)
		{
			//turn right by rotating around y axis
			transform.Rotate(0.0f, turnStrength, 0.0f, Space.Self);
		}

		//tiny negative reward every step to encourage movement
		AddReward(-1f / agentParameters.maxStep);
	}

	public override void AgentReset()
	{
		tankArea.ResetArea();
	}

	public override void CollectObservations()
	{
		//direction tank is facing
		AddVectorObs(transform.forward);
	}


	//we use fixed update function to avoid things breaking when game is sped up for training
	private void FixedUpdate()
	{

	}

	//used to play test the agent actions using keyboard input
	public override float[] Heuristic()
	{

		float[] playerInput = { 0f, 0f };

		if (Input.GetKey(KeyCode.W))
		{
			playerInput[0] = 1;
		}
		if (Input.GetKey(KeyCode.S))
		{
			playerInput[0] = 2;
		}
		if (Input.GetKey(KeyCode.A))
		{
			playerInput[1] = 1;
		}
		if (Input.GetKey(KeyCode.D))
		{
			playerInput[1] = 2;
		}
		return playerInput;
	}
}

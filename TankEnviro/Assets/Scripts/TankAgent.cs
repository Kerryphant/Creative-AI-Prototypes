using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


public class TankAgent : Agent
{
	//area the tank is training in
	private TankArea tankArea;
	
	//prefab for the projectile the tank will shoot
	public Projectile projectilePrefab;

	//list of active projectiles
	private List<GameObject> projectileList;

	//tank's rigid body
	Rigidbody rigidbody;

	Collider collider;

	//tank acceleration
	private const float acceleration = 0.3f;
	//tank turn speed
	private const float turnStrength = 0.6f;

	private void Start()
	{
		tankArea = GetComponentInParent<TankArea>();
		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();
	}

	public override void AgentAction(float[] vectorAction)
	{
		//convert actions to axis values
		//MOVE FORWARD/BACKWARD - this needs to be changed as the tank slides when it shouldn't
		if (vectorAction[0] == 1f)
		{
			//move forwards
			rigidbody.AddForce(transform.forward * acceleration, ForceMode.VelocityChange);
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
			transform.RotateAround(transform.GetChild(0).GetComponent<Collider>().bounds.center, new Vector3(0, 1, 0), -turnStrength);
		}
		else if (vectorAction[1] == 2f)
		{
			//turn right by rotating around y axis
			transform.RotateAround(transform.GetChild(0).GetComponent<Collider>().bounds.center, new Vector3(0, 1, 0), turnStrength);
		}

		if (vectorAction[2] == 1f)
		{
			Shoot();
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

	private void Shoot()
	{
		Projectile projectile = Instantiate<Projectile>(projectilePrefab);
		projectile.transform.parent = transform.parent;
		projectile.transform.position = transform.position + (transform.forward * 4);

		projectile.GetComponent<Rigidbody>().AddForce(transform.forward * 50, ForceMode.VelocityChange);
	}

	//used to play test the agent actions using keyboard input
	public override float[] Heuristic()
	{

		float[] playerInput = { 0f, 0f, 0f};

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

		if (Input.GetKey(KeyCode.J))
		{
			playerInput[2] = 1;
		}
		return playerInput;
	}
}

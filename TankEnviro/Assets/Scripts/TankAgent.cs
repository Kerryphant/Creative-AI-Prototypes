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
	
	//Used to check if the Tank is able to shoot
	public bool readyShoot = true;
	public float timeSinceShoot;

	public float health = 15;
	Vector3 startingPos;

	//tank acceleration
	private const float acceleration = 0.3f;
	//tank turn speed
	private const float turnStrength = 0.6f;

	private void Start()
	{
		tankArea = GetComponentInParent<TankArea>();
		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();

		startingPos = transform.position;
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
		transform.position = TankArea.ChooseRandomPosition(startingPos, 5) + new Vector3(0, 0, 7);
		transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
	}

	public override void CollectObservations()
	{
		//direction tank is facing
		AddVectorObs(transform.forward);

		AddVectorObs(readyShoot);

		AddVectorObs(health);

		var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		AddVectorObs(localVelocity.x);
		AddVectorObs(localVelocity.z);
	}

	//we use fixed update function to avoid things breaking when game is sped up for training
	private void FixedUpdate()
	{

		if(health <= 0)
		{
			AddReward(-10);
			Destroy(gameObject);
		}

		timeSinceShoot += Time.fixedDeltaTime;

		if (timeSinceShoot > 2)
		{
			readyShoot = true;
		}
		else 
		{
			readyShoot = false;
		}
	}

	public void HitEnemy() 
	{
		AddReward(1);
	}

	public void Shoot()
	{
		if (readyShoot) 
		{
			Projectile projectile = Instantiate<Projectile>(projectilePrefab);
			projectile.transform.parent = transform.parent;
			projectile.transform.position = transform.position + (transform.forward * 2) + (transform.up * 1.5f);

			projectile.setOwner(gameObject);


			projectile.GetComponent<Rigidbody>().AddForce(transform.forward * 30, ForceMode.VelocityChange);

			timeSinceShoot = 0;
			readyShoot = false;
		}
	}

	public void TakeDamage(float dmg_value)
	{
		//check damage vaue is s positive number to avoid "healing"
		if(dmg_value >= 0)
		{
			health -= dmg_value;
			//negative reward to encourage avoiding damage
			AddReward(-1f);
		}
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


public class TankAgent : Agent
{
	public Projectile projectilePrefab; //prefab for the projectile the tank will shoot
	private TankArea tankArea; //area the tank is in
	private List<GameObject> projectileList; //list of active projectiles

	Rigidbody rigidbody;
	Collider collider;
	
	public bool readyShoot = true; // if the Tank is able to shoot
	public float timeSinceShoot;
	public float health = 15;
	private Vector3 startingPos;
	private const float turnStrength = 0.6f;    //tank turn speed
	private float spawnRadius = 0.0f;


    public AK.Wwise.Event Fire = new AK.Wwise.Event();
    public AK.Wwise.Event Death = new AK.Wwise.Event();


    private void Start()
	{
		tankArea = GetComponentInParent<TankArea>();
		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();

		startingPos = transform.position;
	}

	public override void AgentAction(float[] vectorAction)
	{
		//tank movement
		if (vectorAction[0] == 1f)
		{
			//move forwards
			transform.position += (transform.forward * Time.deltaTime);
		}

		//tank turning
		if (vectorAction[1] == 1f)
		{
			//turn left by rotating around y axis
			transform.RotateAround(transform.GetComponent<Collider>().bounds.center, new Vector3(0, 1, 0), -turnStrength);
		}
		else if (vectorAction[1] == 2f)
		{
			//turn right by rotating around y axis
			transform.RotateAround(transform.GetComponent<Collider>().bounds.center, new Vector3(0, 1, 0), turnStrength);
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
		//tankArea.ResetArea();
		transform.position = TankArea.ChooseRandomPosition(startingPos, spawnRadius, spawnRadius);
		transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
	}

	public override void CollectObservations()
	{
		//direction tank is facing
		AddVectorObs(transform.forward);

		//direction to the closest enemy
		GameObject closestEnemy = FindClosestEnemy();
		AddVectorObs((closestEnemy.transform.position - transform.position).normalized);

		//able to shoot?
		AddVectorObs(readyShoot);

		//x and z velocities
		var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		AddVectorObs(localVelocity.x);
		AddVectorObs(localVelocity.z);
	}

	//taken from https://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html
	public GameObject FindClosestEnemy()
	{
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("tankAgent");
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in gos)
		{
			if(go.transform == this.transform)
			{
				continue;
			}
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance)
			{
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}

	//we use fixed update function to avoid things breaking when game is sped up for training
	private void FixedUpdate()
	{
		//Reward for looking at an enemy straight on
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 300) && hit.transform.tag == "tankAgent")
		{
			AddReward((1f / agentParameters.maxStep)*3);
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
		}

		//check if the tank needs to die and "respawn"
		if (health <= 0)
		{
			health = 15;



            Death.Post(gameObject);

            transform.position = TankArea.ChooseRandomPosition(startingPos, spawnRadius, spawnRadius);// + new Vector3(0, 0, 7);
			transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		}

		//update varibles tracking ability to shoot
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

	public void AmmoWaste()
	{
		//small negative reward so to not entirely discourage shooting
		AddReward(-0.25f);
	}

	public void Shoot()
	{
		if (readyShoot) 
		{
			//creat bullet with a position slightly forward and above the tank, use the tanks rotation
			Projectile projectile = Instantiate<Projectile>(projectilePrefab, (transform.position + (transform.forward * 2) + (transform.up * 1.5f)), this.transform.rotation);

			//set up the onwer variable to allow for callback
			projectile.setOwner(gameObject);

            //wwise event for gun shot
            Fire.Post(gameObject);

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

			//This is not the correct way to train avoidance of damage.
			//The tank will receive the negative reward without "understanding" where it came from

			//negative reward to encourage avoiding damage
			//AddReward(-1f);
		}
	}

	public void setSpawnRadius(float spawnRadius_)
	{
		spawnRadius = spawnRadius_;
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

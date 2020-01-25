using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public float enemySpeed;

	private float randomizedSpeed = 0f;
	private float nextActionTime = -1f;
	private Vector3 targetPosition;

	private void FixedUpdate()
	{
		if (enemySpeed > 0)
		{
			Move();
		}
	}

	private void Move()
	{
		if (Time.fixedTime >= nextActionTime)
		{
			//randomise the speed
			randomizedSpeed = enemySpeed * UnityEngine.Random.Range(.5f, 1.5f);

			//pick a random target
			targetPosition = TankArea.ChooseRandomPosition(transform.parent.position, 15) + new Vector3(0, 0, 12);

			//rotate toward the target
			transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

			//calc time to get to target
			float timeToGetThere = Vector3.Distance(transform.position, targetPosition) / randomizedSpeed;
			nextActionTime = Time.fixedTime + timeToGetThere;
		}
		else
		{
			//make sure that the tank does not move past the target
			Vector3 moveVector = randomizedSpeed * transform.forward * Time.fixedDeltaTime;
			if (moveVector.magnitude <= Vector3.Distance(transform.position, targetPosition))
			{
				transform.position += moveVector;
			}
			else
			{
				transform.position = targetPosition;
				nextActionTime = Time.fixedTime;
			}
		}
	}
}

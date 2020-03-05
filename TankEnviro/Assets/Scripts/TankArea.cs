using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using TMPro;
using System;

public class TankArea : Area
{
    public TankAgent tankAgent;
    public Enemy enemyPrefab;
    public TextMeshPro cumulativeRewardText;

    [HideInInspector]
    public float enemySpeed = 0f;
    [HideInInspector]
    public float turretRange = 0f;

    private List<GameObject> enemyList;

    //resets the area at the end of a lesson
    public override void ResetArea()
    {
        RemoveEnemies();
        PlaceTank();
        //PlaceEnemies(3, enemySpeed);
    }

    //removes all of the enemies in the area from the enemyList
    private void RemoveEnemies()
	{
        if (enemyList != null)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i] != null)
                {
                    Destroy(enemyList[i]);
                }
            }
        }

        enemyList = new List<GameObject>();
    }
    //places the player tank randomly within an area
    private void PlaceTank()
	{
        //tankAgent.transform.position = ChooseRandomPosition(transform.position, 15) + new Vector3(0, 0, 7);
        //tankAgent.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }

    //places the enemy tanks randomly within an area
    private void PlaceEnemies(int spawnCount_, float enemySpeed_)
    {
        for(int i = 0; i < spawnCount_; i++)
		{
            //create the new object and store in a temporary variable
            GameObject enemyObject = Instantiate<GameObject>(enemyPrefab.gameObject);

            //set the position of the new instance
            enemyObject.transform.position = ChooseRandomPosition(transform.position, 15) + new Vector3(0, 0, 7);
            enemyObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            enemyObject.transform.parent = transform;

            //add the new enemy to the list of enemies
            enemyList.Add(enemyObject);

            //set the enemy speed
            //enemyObject.GetComponent<Enemy>().enemySpeed = enemySpeed;

        }
    }

    //could change size to be a vector3 instead of fixed value
    public static Vector3 ChooseRandomPosition(Vector3 center, int size_)
    {
        return center + new Vector3( (UnityEngine.Random.value - 0.5f) * size_, 0.5f, (UnityEngine.Random.value - 0.5f) * size_);
    }

    private void Update()
    {
        //update the text inside the scene to reflect the agent's current reward score
        cumulativeRewardText.text = tankAgent.GetCumulativeReward().ToString("0.00");
    }

}
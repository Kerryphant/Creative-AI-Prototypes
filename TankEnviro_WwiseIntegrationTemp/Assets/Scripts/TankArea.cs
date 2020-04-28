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
    public float spawnRadius = 0f;

    //resets the area at the end of a lesson
    public override void ResetArea()
    {
        tankAgent.setSpawnRadius(spawnRadius);
    }

    //places the enemy tanks randomly within an area
    /*private void PlaceEnemies(int spawnCount_, float enemySpeed_)
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

        }
    }*/

    //chooses a random position within a square on the Z and X axis
    public static Vector3 ChooseRandomPosition(Vector3 center, float halfXDistance, float halfZDistance)
    {
        //return center + new Vector3( (UnityEngine.Random.value - 0.5f) * size_, 0.5f, (UnityEngine.Random.value - 0.5f) * size_);
        float minX = center.x - halfXDistance;
        float maxX = center.x + halfXDistance;
        float minZ = center.z - halfZDistance;
        float maxZ = center.z + halfZDistance;

        return new Vector3(UnityEngine.Random.Range(minX, maxX), 0.5f, UnityEngine.Random.Range(minZ, maxZ));
    }

    private void Update()
    {
        //update the text inside the scene to reflect the agent's current reward score
        cumulativeRewardText.text = tankAgent.GetCumulativeReward().ToString("0.00");
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class TankAcademy : Academy
{

    private TankArea[] tankAreas;

    public override void AcademyReset()
    {
        //get the tank areas
        if (tankAreas == null)
        {
            tankAreas = FindObjectsOfType<TankArea>();
        }

        //set up areas
        foreach (TankArea tankArea in tankAreas)
        {
            //set up values from curriculum
            FloatProperties.RegisterCallback("enemy_speed", f =>
            {
                tankArea.enemySpeed = f;
            });

            FloatProperties.RegisterCallback("turret_range", f =>
            {
                tankArea.turretRange = f;
            });

            //reset objects inside the area
            tankArea.ResetArea();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[4];
    [SerializeField] private GameObject[] wheelMeshes = new GameObject[4];
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float fullTorque;
    [SerializeField] private float reverseTorque;
    [SerializeField] private float brakeTorque;
    [SerializeField] private float handbrakeTorque;
    [SerializeField] private float downForce = 120f;
    [SerializeField] private float topSpeed = 300f;
    [Range(0, 1)] [SerializeField] private float steerHelp;

    public Collider carCollider;

    Quaternion[] wheelMeshRotations;
    Rigidbody rigidbody;
    float steerAngle;
    float currentTorque;
    float oldRotation;

    //Getter for current speed (to be used for UI, etc)
    public float CurrentSpeed 
    { 
        get { return rigidbody.velocity.magnitude * 2.23693629f; } 
    }
    //Setting up all the variables
    private void Start()
    {
        wheelMeshRotations = new Quaternion[4];

        for(int i = 0; i < 4; i++)
        {
            wheelMeshRotations[i] = wheelMeshes[i].transform.localRotation;
        }

        handbrakeTorque = float.MaxValue;

        rigidbody = GetComponent<Rigidbody>();
        currentTorque = fullTorque;
           
        Physics.IgnoreLayerCollision(9, 9);
    }

    public void Move(float steer, float acceleration, float brake, bool handbrake)
    {
        //Update mesh positions to match that of colliders
        //Unity doesn't seem to like having the meshes and colliders in the same object so this is neccesary
        for (int i = 0; i < 4; i++)
        {
           // Quaternion q;
           // Vector3 pos;

           // wheelColliders[i].GetWorldPose(out pos, out q);
           //wheelMeshes[i].transform.position = pos;
           // wheelMeshes[i].transform.rotation = q;
        }

        //Clamp all the input values
        steer = Mathf.Clamp(steer, -1, 1);
        acceleration = Mathf.Clamp(acceleration, 0, 1);
        brake = Mathf.Clamp(brake, 0, 1);

        //Set the front wheels to steer
        //0 and 1 are the front wheels in the array
        steerAngle = steer * maxSteerAngle;
        wheelColliders[0].steerAngle = steerAngle;
        wheelColliders[1].steerAngle = steerAngle;

        SteerHelp();
        ApplyDrive(acceleration, brake);
        CapSpeed();

        //Apply the handbrake
        //3 and 4 should be the back wheels in the array
        if(handbrake == true)
        {
            wheelColliders[2].brakeTorque = handbrakeTorque;
            wheelColliders[3].brakeTorque = handbrakeTorque;
        }

        DownForce();
    }

    public void Reset()
    {
        rigidbody.velocity = Vector3.zero;
    }

    void ApplyDrive(float acceleration, float brake)
    {
        //Apply torque to the rear wheels to create thrust
        float thrustTorque = acceleration * (currentTorque / 2f);
        wheelColliders[2].motorTorque = wheelColliders[3].motorTorque = thrustTorque;

        
        //Apply footbrake / reverse
        for (int i = 0; i < 4; i++)
        {
            //Checking the car is moving forward
            if(CurrentSpeed > 3 && Vector3.Angle(transform.forward, rigidbody.velocity) < 45f)
            {
                wheelColliders[i].brakeTorque = brakeTorque * brake;

                //Debug.Log(wheelColliders[i].brakeTorque);
            } else if (brake > 0)
            {
                wheelColliders[i].brakeTorque = 0f;
                wheelColliders[i].motorTorque = reverseTorque * brake;
            }
        }
    }

    //Make sure the car doesn't go over maxspeed
    void CapSpeed()
    {
        float speed = rigidbody.velocity.magnitude;

        speed *= 2.23693629f;   //Convert velocity magnitude to MPH (MPS to MPH value)
        if (speed > topSpeed)
            rigidbody.velocity = ((topSpeed / 2.23693629f) * rigidbody.velocity.normalized);
    }

    void DownForce()
    {
        rigidbody.AddForce(-transform.up * downForce * rigidbody.velocity.magnitude);
    }

    void SteerHelp()
    {
        for(int i = 0; i < 4; i++)
        {
            WheelHit hit;
            wheelColliders[i].GetGroundHit(out hit);
            if (hit.normal == Vector3.zero)
                return; //No need to realign vectors if the wheel isn't on the ground
        }

        if(Mathf.Abs(oldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnAdjust = (transform.eulerAngles.y - oldRotation) * steerHelp;
            Quaternion velRotation = Quaternion.AngleAxis(turnAdjust, Vector3.up);
            rigidbody.velocity = velRotation * rigidbody.velocity;
        }

        oldRotation = transform.eulerAngles.y;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject);

        if(collision.gameObject.tag == "Car")
        {
            Physics.IgnoreCollision(collision.collider, carCollider, true);
        }

        if (collision.gameObject.tag == "Wall")
        {
            CarAgent agent = gameObject.GetComponent<CarAgent>();
            if (agent != null)
            {
                //agent.AddReward(-100);
                agent.Done();
            }
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            CarAgent agent = gameObject.GetComponent<CarAgent>();
            if (agent != null)
                agent.AddReward(-0.10f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    Rigidbody rigidbody;

    private TankAgent owner;

    void Start()
	{
        Destroy(this.gameObject, 3);
    }

    public void setOwner(GameObject owner_)
	{
        owner = owner_.GetComponent(typeof(TankAgent)) as TankAgent;
    }

    void OnCollisionEnter(Collision collision)
    { 

        if (collision.gameObject.tag == "enemyTank")
        {
            //If the GameObject has the same tag as specified, output this message in the console
            Debug.Log("Hit enemy!");


            owner.HitEnemy();
        }

        if (collision.gameObject.tag == "tankAgent")
        {
            //If the GameObject has the same tag as specified, output this message in the console
            Debug.Log("Hit tank agent!");

            owner.HitEnemy();

            TankAgent hitTank = collision.gameObject.GetComponent<TankAgent>();

            hitTank.TakeDamage(5);
        }

        //collision.gameObject.tag != "Untagged" && 
        Destroy(this.gameObject);

        Debug.Log("Collided with " + collision.gameObject.tag);
    }
}

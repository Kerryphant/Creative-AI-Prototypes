using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    Rigidbody rigidbody;

    private TankAgent owner;

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

        Debug.Log("Collided with " + collision.gameObject.tag);

        if (collision.gameObject.tag != "Untagged" && collision.gameObject.tag != "floor")
        {
            Destroy(this.gameObject);
        }
    }
}

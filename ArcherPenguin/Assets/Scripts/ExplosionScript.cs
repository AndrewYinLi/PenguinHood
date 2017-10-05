using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour {
    private float time = 0.3f;
    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if ((time -= Time.deltaTime) <= 0) // Countdown to explosion
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player") // If player is caught in the explosion radius, change the player's sprite animation to jumping because the player is no longer grounded
        {
            if (col.GetComponent<PlayerController>().currentAnimationState != 3 && col.GetComponent<PlayerController>().currentAnimationState != 4)
            {
                col.GetComponent<PlayerController>().changeState(2);
            }
        }
        Vector3 dir = col.transform.position - transform.position; // determining direction that object should be pushed
        dir = dir.normalized; // make the direction a unit vector
        try
        {
            col.GetComponent<Rigidbody2D>().AddForce(dir * 11000); // add force
        }
        catch (Exception e) { }
    }
}

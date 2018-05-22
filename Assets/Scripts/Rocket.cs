using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision) {
        foreach (Collider c in Physics.OverlapSphere(transform.position, 10f)) {
            Rigidbody rb = c.GetComponent<Rigidbody>();
            if (rb) {
                rb.AddExplosionForce(1000f, transform.position, 10f);
            }
        }

        Destroy(this.gameObject);
    }
}

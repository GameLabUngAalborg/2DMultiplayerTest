using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BulletScript : NetworkBehaviour
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void OnCollisionEnter2D (Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            // Kør TakeDamage script på den spiller vi rammer

            other.gameObject.GetComponent<PlayerScript>().TakeDamage(10);
            // Fortæl derefter at dette objekt skal destroyes til networkserveren. Den vil sørge for at despawn den på alle computere.
            NetworkServer.Destroy(gameObject);
            
        }
	}
}
